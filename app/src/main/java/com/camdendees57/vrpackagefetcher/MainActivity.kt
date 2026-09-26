package TagtusVR.IsCool

import android.app.DownloadManager
import android.content.Context
import android.content.Intent
import android.net.Uri
import android.os.Bundle
import android.os.Environment
import android.widget.*
import androidx.appcompat.app.AppCompatActivity

class MainActivity : AppCompatActivity() {
    private lateinit var apkUrl: EditText
    private lateinit var obbUrl: EditText
    private lateinit var gameSearch: EditText
    private lateinit var status: TextView

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        val root = LinearLayout(this).apply {
            orientation = LinearLayout.VERTICAL
            setPadding(32, 32, 32, 32)
        }

        val title = TextView(this).apply {
            text = "Apk/Obb Downloader"
            textSize = 26f
        }

        val subtitle = TextView(this).apply {
            text = "Find Meta Quest games, then download authorized APK + optional OBB files."
            setPadding(0, 8, 0, 24)
        }

        val searchTitle = TextView(this).apply {
            text = "Meta Horizon Store Search"
            textSize = 18f
        }

        gameSearch = EditText(this).apply {
            hint = "Game name, e.g. Beat Saber"
            inputType = android.text.InputType.TYPE_CLASS_TEXT
        }

        val search = Button(this).apply {
            text = "Search Meta Store"
            setOnClickListener { searchMetaStore() }
        }

        val divider = TextView(this).apply {
            text = "\nAuthorized direct-download URLs"
            textSize = 18f
        }

        apkUrl = EditText(this).apply {
            hint = "APK direct-download URL"
            inputType = android.text.InputType.TYPE_CLASS_TEXT or
                android.text.InputType.TYPE_TEXT_VARIATION_URI
        }

        obbUrl = EditText(this).apply {
            hint = "Optional OBB direct-download URL"
            inputType = android.text.InputType.TYPE_CLASS_TEXT or
                android.text.InputType.TYPE_TEXT_VARIATION_URI
        }

        val fetch = Button(this).apply {
            text = "Fetch package"
            setOnClickListener { fetchPackage() }
        }

        status = TextView(this).apply {
            text = "Files are saved to Downloads."
            setPadding(0, 20, 0, 0)
        }

        root.addView(title)
        root.addView(subtitle)
        root.addView(searchTitle)
        root.addView(gameSearch)
        root.addView(search)
        root.addView(divider)
        root.addView(apkUrl)
        root.addView(obbUrl)
        root.addView(fetch)
        root.addView(status)

        setContentView(root)
    }

    private fun searchMetaStore() {
        val query = gameSearch.text.toString().trim()
        if (query.isEmpty()) {
            status.text = "Enter a game name first."
            return
        }

        val url = "https://www.meta.com/experiences/?q=" + Uri.encode(query)
        val intent = Intent(Intent.ACTION_VIEW, Uri.parse(url))

        try {
            startActivity(intent)
            status.text = "Opened Meta Horizon Store search for: " + query
        } catch (_: Exception) {
            status.text = "Could not open the Meta Horizon Store."
        }
    }

    private fun fetchPackage() {
        val apk = apkUrl.text.toString().trim()
        val obb = obbUrl.text.toString().trim()

        if (!apk.startsWith("https://")) {
            status.text = "Enter a valid HTTPS APK URL."
            return
        }

        enqueue(apk, "vr-package.apk")

        if (obb.isNotEmpty()) {
            if (!obb.startsWith("https://")) {
                status.text = "The OBB URL must use HTTPS."
                return
            }
            enqueue(obb, "vr-expansion.obb")
        }

        status.text = if (obb.isEmpty()) {
            "APK download queued."
        } else {
            "APK + OBB downloads queued."
        }
    }

    private fun enqueue(url: String, filename: String) {
        val request = DownloadManager.Request(Uri.parse(url))
            .setTitle(filename)
            .setDescription("Apk/Obb Downloader")
            .setNotificationVisibility(
                DownloadManager.Request.VISIBILITY_VISIBLE_NOTIFY_COMPLETED
            )
            .setDestinationInExternalPublicDir(
                Environment.DIRECTORY_DOWNLOADS,
                filename
            )

        (getSystemService(Context.DOWNLOAD_SERVICE) as DownloadManager).enqueue(request)
    }
}
