package nl.kaouch.jaouad.comakership.login

import android.content.Context
import android.content.SharedPreferences

class TokenManager(conText: Context) {

    private var Mode = 0
    private val RENAME = "JWTTOKEN"
    private val KEY_USER_ID = "ID"
    private val KEY_JWT_TOKEN = "JWTTOKEN"
    private var sharedPreferences = conText.getSharedPreferences(RENAME, Mode)
    private var editor: SharedPreferences.Editor = sharedPreferences.edit()

    fun createSession(userId: String, jwtValue: String) {
        editor.putString(KEY_USER_ID, userId)
        editor.putString(KEY_JWT_TOKEN, jwtValue)
        editor.commit()
    }

    fun getToken(): String {
        var token = sharedPreferences.getString(KEY_JWT_TOKEN, "")
        return token!!
    }

    fun getUserId(): Int {
        var userId = sharedPreferences.getString(KEY_USER_ID, "")
        return userId!!.toInt()
    }

    fun clearJwtToken() {
        editor.remove(KEY_USER_ID)
        editor.remove(KEY_JWT_TOKEN)
        editor.commit()
    }

}