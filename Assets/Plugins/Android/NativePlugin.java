package com.example.nativeplugin;

import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.widget.Toast;
import java.util.ArrayList;
import android.util.Log;
import java.io.File; 
public class NativePlugin {
public static void showToast(Activity activity, String message, boolean isLongDuration) {
    activity.runOnUiThread(() -> {
        try {
            int duration = isLongDuration ? Toast.LENGTH_LONG : Toast.LENGTH_SHORT;
            Toast.makeText(activity, message, duration).show();
        } catch (Exception e) {
            Toast.makeText(activity, "Ошибка отображения тоста: " + e.getMessage(), Toast.LENGTH_SHORT).show();
            Log.e("NativePlugin", "Ошибка отображения тоста", e);
        }
    });
}
    public static void rateApp(Activity activity) {
        try {
            String packageName = activity.getPackageName();
            Uri uri = Uri.parse("market://details?id=" + packageName);
            Intent intent = new Intent(Intent.ACTION_VIEW, uri);
            intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            activity.startActivity(intent);
        } catch (Exception e) {
            Uri uri = Uri.parse("https://play.google.com/store/apps/details?id=" + activity.getPackageName());
            Intent intent = new Intent(Intent.ACTION_VIEW, uri);
            intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            activity.startActivity(intent);
        }
    }

    public static void shareText(Activity activity, String text, String subject) {
        try {
            Intent intent = new Intent(Intent.ACTION_SEND);
            intent.setType("text/plain");
            if (subject != null && !subject.isEmpty()) {
                intent.putExtra(Intent.EXTRA_SUBJECT, subject);
            }
            intent.putExtra(Intent.EXTRA_TEXT, text);
            intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            activity.startActivity(Intent.createChooser(intent, "Share via"));
        } catch (Exception e) {
            activity.runOnUiThread(() -> {
                Toast.makeText(activity, "No app found to share content", Toast.LENGTH_SHORT).show();
            });
        }
    }

    public static void openEmail(Activity activity, String emailAddress, String subject, String body) {
        Intent intent = new Intent(Intent.ACTION_SENDTO);
        intent.setData(Uri.parse("mailto:" + emailAddress));
        if (subject != null && !subject.isEmpty()) {
            intent.putExtra(Intent.EXTRA_SUBJECT, subject);
        }
        if (body != null && !body.isEmpty()) {
            intent.putExtra(Intent.EXTRA_TEXT, body);
        }
        intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
        try {
            activity.startActivity(Intent.createChooser(intent, "Send email"));
        } catch (Exception e) {
            activity.runOnUiThread(() -> {
                Toast.makeText(activity, "No email app found", Toast.LENGTH_SHORT).show();
            });
        }
    }

     public static void shareImage(Activity activity, String imagePath, String text, String subject) {
        try {
            Intent intent = new Intent(Intent.ACTION_SEND);
            intent.setType("image/*");
            
            if (imagePath != null && !imagePath.isEmpty()) {
                Uri imageUri = Uri.parse(imagePath);
                intent.putExtra(Intent.EXTRA_STREAM, imageUri);
            }
            
            if (text != null && !text.isEmpty()) {
                intent.putExtra(Intent.EXTRA_TEXT, text);
            }
            
            if (subject != null && !subject.isEmpty()) {
                intent.putExtra(Intent.EXTRA_SUBJECT, subject);
            }
            
            intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            intent.addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);
            activity.startActivity(Intent.createChooser(intent, "Share image via"));
        } catch (Exception e) {
            activity.runOnUiThread(() -> {
                Toast.makeText(activity, "No app found to share image", Toast.LENGTH_SHORT).show();
            });
        }
    }

    public static void openEmailMultiple(Activity activity, ArrayList<String> emailAddresses, String subject, String body) {
        try {
            Intent intent = new Intent(Intent.ACTION_SENDTO);
            intent.setData(Uri.parse("mailto:"));
            intent.putExtra(Intent.EXTRA_EMAIL, emailAddresses.toArray(new String[0]));
            
            if (subject != null && !subject.isEmpty()) {
                intent.putExtra(Intent.EXTRA_SUBJECT, subject);
            }
            
            if (body != null && !body.isEmpty()) {
                intent.putExtra(Intent.EXTRA_TEXT, body);
            }
            
            intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            activity.startActivity(Intent.createChooser(intent, "Send email"));
        } catch (Exception e) {
            activity.runOnUiThread(() -> {
                Toast.makeText(activity, "No email app found", Toast.LENGTH_SHORT).show();
            });
        }
    }

public static void shareFile(Activity activity, String filePath, String text, String subject) {
    try {
        Intent intent = new Intent(Intent.ACTION_SEND);
        intent.setType("text/csv"); // MIME-тип для CSV

        if (filePath != null && !filePath.isEmpty()) {
            Uri fileUri = Uri.parse(filePath);
            intent.putExtra(Intent.EXTRA_STREAM, fileUri);
        }

        if (text != null && !text.isEmpty()) {
            intent.putExtra(Intent.EXTRA_TEXT, text);
        }

        if (subject != null && !subject.isEmpty()) {
            intent.putExtra(Intent.EXTRA_SUBJECT, subject);
        }

        intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
        intent.addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);
        activity.startActivity(Intent.createChooser(intent, "Поделиться файлом через"));
    } catch (Exception e) {
        activity.runOnUiThread(() -> {
            Toast.makeText(activity, "Нет приложений для отправки файла", Toast.LENGTH_SHORT).show();
            Log.e("NativePlugin", "Ошибка отправки файла: " + e.getMessage(), e);
        });
    }
}


}