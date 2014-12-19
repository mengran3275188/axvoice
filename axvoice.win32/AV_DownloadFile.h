#pragma once

class DownloadFile
{
public:
	typedef void (CALLBACK *ON_COMPLETE_CALLBACK)(unsigned int, bool, const std::string&);  

	bool beginDownloadThread(void);

private:
	static UINT CALLBACK _downloadThread(void* pParam);
	void downloadThread(void);
	bool _writeLocalFile(void);

private:
	ON_COMPLETE_CALLBACK m_completeCallback;
	unsigned int m_voiceID;
	std::string m_downloadURL;
	std::string m_localFile;
	HANDLE m_hThread;
	std::string m_md5;

private:
	static size_t _curlWriteMemory(void *ptr, size_t size, size_t nmemb, void *data);
	unsigned char* m_curlTempMemory;	//!< ���ڽ��ս������ʱ�ڴ�
	size_t m_curlTempMemorySize;		//!< ���ڽ��ս������ʱ�ڴ泤��
	size_t m_curlTempMemoryTail;		//!< �Ѿ�ʹ�õ���ʱ�ڴ泤��

public:
	DownloadFile(unsigned int voiceID, 
		const std::string& downloadURL, const std::string& localFile, 
		ON_COMPLETE_CALLBACK cbComplete);
	~DownloadFile();
};
