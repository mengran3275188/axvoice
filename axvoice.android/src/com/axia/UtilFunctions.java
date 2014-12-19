package com.axia;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.security.MessageDigest;

import android.os.Environment;

public class UtilFunctions {

	static char hexdigits[] = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
	
	public static String getSDPath() 
	{
		String sdDir = "";
		// is sdcard exist
		boolean sdCardExist = Environment.getExternalStorageState().equals(Environment.MEDIA_MOUNTED); 
		
		if (sdCardExist) {
			sdDir = Environment.getExternalStorageDirectory() + "/changyou/";
		} else {
			sdDir = "/data/data/com.axia.voice/changyou/";
		}

		File file = new File(sdDir);
		if (!file.exists()) {
			file.mkdirs();
		}
		return sdDir;
	}
	
	private static String byteToHexString(byte[] tmp) {
		String s;
		// ���ֽڱ�ʾ���� 16 ���ֽ�
		char str[] = new char[16 * 2]; // ÿ���ֽ��� 16 ���Ʊ�ʾ�Ļ���ʹ�������ַ���
		// ���Ա�ʾ�� 16 ������Ҫ 32 ���ַ�
		int k = 0; // ��ʾת������ж�Ӧ���ַ�λ��
		for (int i = 0; i < 16; i++) { // �ӵ�һ���ֽڿ�ʼ���� MD5 ��ÿһ���ֽ�
			// ת���� 16 �����ַ���ת��
			byte byte0 = tmp[i]; // ȡ�� i ���ֽ�
			str[k++] = hexdigits[byte0 >>> 4 & 0xf]; // ȡ�ֽ��и� 4 λ������ת��,
			// >>> Ϊ�߼����ƣ�������λһ������
			str[k++] = hexdigits[byte0 & 0xf]; // ȡ�ֽ��е� 4 λ������ת��
		}
		s = new String(str); // ����Ľ��ת��Ϊ�ַ���
		return s;
	}
	
	public static String getMd5ByFile(File file) {
		FileInputStream fis = null;
		try {
			MessageDigest md = MessageDigest.getInstance("MD5");
			fis = new FileInputStream(file);
			byte[] buffer = new byte[2048];
			int length = -1;
			while ((length = fis.read(buffer)) != -1) {
				md.update(buffer, 0, length);
			}

			byte[] b = md.digest();
			return byteToHexString(b);
		} catch (Exception ex) {
			ex.printStackTrace();
			return null;
		} finally {
			try {
				fis.close();
			} catch (IOException ex) {
				ex.printStackTrace();
			}
		}
	}	
}
