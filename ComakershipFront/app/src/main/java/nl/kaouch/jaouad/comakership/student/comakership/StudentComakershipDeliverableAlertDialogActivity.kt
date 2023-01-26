package nl.kaouch.jaouad.comakership.student.comakership

import android.Manifest
import android.app.Activity
import android.app.AlertDialog
import android.content.Intent
import android.content.pm.PackageManager
import android.net.Uri
import android.os.Build
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.widget.Button
import android.widget.ImageView
import android.widget.Toast
import androidx.activity.result.contract.ActivityResultContracts
import androidx.annotation.RequiresApi
import androidx.appcompat.app.AppCompatActivity
import androidx.core.app.ActivityCompat
import nl.kaouch.jaouad.comakership.API.ApiInterface
import nl.kaouch.jaouad.comakership.BASE_URL
import nl.kaouch.jaouad.comakership.R
import nl.kaouch.jaouad.comakership.getFileName
import nl.kaouch.jaouad.comakership.login.LoginActivity
import nl.kaouch.jaouad.comakership.login.TokenManager
import nl.kaouch.jaouad.comakership.student.dashboard.StudentDashboardActivity
import okhttp3.MediaType
import okhttp3.MultipartBody
import okhttp3.RequestBody
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import java.io.File
import java.io.FileInputStream
import java.io.FileOutputStream


class StudentComakershipDeliverableAlertDialogActivity : AppCompatActivity() {

    private lateinit var filePickBtn: Button
    private lateinit var uploadBtn: Button
    private lateinit var tokenManager: TokenManager
    private lateinit var docFilePath: Uri
    private lateinit var toolbarBackButton: ImageView

    private val REQUEST_EXTERNAL_STORAGE = 1
    private val PERMISSIONS_STORAGE = arrayOf<String>(
        Manifest.permission.READ_EXTERNAL_STORAGE,
        Manifest.permission.WRITE_EXTERNAL_STORAGE
    )

    var resultLauncher = registerForActivityResult(ActivityResultContracts.StartActivityForResult()) { result ->
        if (result.resultCode == Activity.RESULT_OK) {
            val data: Intent? = result.data
            val fileuri: Uri = data!!.data!!
            docFilePath = fileuri
        }
    }

    @RequiresApi(Build.VERSION_CODES.KITKAT)
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        tokenManager = TokenManager(applicationContext)

        displayAlert()

        filePickBtn.setOnClickListener {
            pick()
        }

        uploadBtn.setOnClickListener {
            uploadFile()
        }
    }

    fun pick() {
        verifyStoragePermissions(this@StudentComakershipDeliverableAlertDialogActivity)
        val intent = Intent(Intent.ACTION_GET_CONTENT)
        intent.addCategory(Intent.CATEGORY_OPENABLE)
        intent.type = "application/pdf"
        Intent.createChooser(intent, "Select a file")
        resultLauncher.launch(intent)
    }

    private fun displayAlert() {
        val alertDialogBuilder: AlertDialog.Builder = AlertDialog.Builder(this)
        val inflater: LayoutInflater = this.layoutInflater
        val view = inflater.inflate(R.layout.student_deliverable_upload_dialog, null)
        alertDialogBuilder.setView(view)

        filePickBtn = view.findViewById(R.id.upload_deliverable_btn)
        uploadBtn = view.findViewById(R.id.apply_upload_btn)
        toolbarBackButton = view.findViewById(R.id.toolbar_back_button)

        toolbarBackButton.setOnClickListener {
            val intent = Intent(this@StudentComakershipDeliverableAlertDialogActivity, ComakershipDashboardActivity::class.java)
            this.finish()
            startActivity(intent)
        }
        val alertDialog: AlertDialog = alertDialogBuilder.create()
        alertDialog.show()
    }

    fun verifyStoragePermissions(activity: Activity?) {
        val permission = ActivityCompat.checkSelfPermission(
            activity!!,
            Manifest.permission.WRITE_EXTERNAL_STORAGE
        )
        if (permission != PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(
                activity,
                PERMISSIONS_STORAGE,
                REQUEST_EXTERNAL_STORAGE
            )
        }
    }

    @RequiresApi(Build.VERSION_CODES.KITKAT)
    fun uploadFile() {
        val comakershipId = intent.getIntExtra("chosenComakershipId", 0)
        val parcelFileDescriptor =
            contentResolver.openFileDescriptor(docFilePath!!, "r", null) ?: return
        val inputStream = FileInputStream(parcelFileDescriptor.fileDescriptor)
        val file = File(cacheDir, contentResolver.getFileName(docFilePath!!))

        val outputStream = FileOutputStream(file)
        inputStream.copyTo(outputStream)

        val reqBody: RequestBody =
            RequestBody.create(MediaType.parse("multipart/form-file"), file)
        val partImage = MultipartBody.Part.createFormData("file", file.name, reqBody)

        val call = fetchApi().uploadDeliverable("Bearer "+tokenManager.getToken(), comakershipId, partImage)
        call.enqueue(object : Callback<Void> {
            override fun onResponse(
                call: Call<Void>,
                response: Response<Void>
            ) {
                if(response.code() == 401) {
                    tokenManager.clearJwtToken()
                    val intent = Intent(this@StudentComakershipDeliverableAlertDialogActivity, LoginActivity::class.java)
                    finish()
                    startActivity(intent)
                }
                if (response.isSuccessful) {
                    Toast.makeText(this@StudentComakershipDeliverableAlertDialogActivity, "Image Uploaded", Toast.LENGTH_SHORT).show()
                    val main = Intent(this@StudentComakershipDeliverableAlertDialogActivity, StudentDashboardActivity::class.java)
                    main.flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
                    startActivity(main)
                }
            }
            override fun onFailure(call: Call<Void>, t: Throwable) {
                Log.e("HTTP", "Could not fetch data", t)
            }
        })
    }

    fun fetchApi(): ApiInterface {
        val retrofitBuilder = Retrofit.Builder()
            .baseUrl(BASE_URL)
            .addConverterFactory(GsonConverterFactory.create())
            .build()
            .create(ApiInterface::class.java)

        val service: ApiInterface = retrofitBuilder
        return service
    }
}