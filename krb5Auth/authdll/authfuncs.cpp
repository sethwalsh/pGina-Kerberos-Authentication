#include "authfuncs.h"

extern "C"{
	__declspec(dllexport) int auth_user(char* u, char* p, char* d, char* t)
	{
		/*
			Creates a new SEC_WINNT_AUTH_IDENTITY structure using the given user name, password, and domain.  These fields are supplied by pGina, and the configuration
			tool for the krb5 plugin.
		*/
		SEC_WINNT_AUTH_IDENTITY auth;
		auth.Domain = reinterpret_cast<unsigned char*>( d );
		//int _d = (d[2] << 8) | d[3];
		//auth.Domain = (unsigned short*)_d;
		auth.DomainLength = strlen( d );
		
		auth.User = reinterpret_cast<unsigned char*>( u );
		//int _u = (u[2] << 8) | u[3];
		//auth.User = (unsigned short*)_u;
		auth.UserLength = strlen( u );
		
		auth.Password = reinterpret_cast<unsigned char*>( p );
		//int _p = (p[2] << 8) | u[3];
		//auth.Password = (unsigned short*)_p;
		auth.PasswordLength = strlen( p );

		auth.Flags = SEC_WINNT_AUTH_IDENTITY_ANSI;

		char clientOutBufferData[8192];
		char serverOutBufferData[8192];

		SecBuffer     clientOutBuffer;
		SecBufferDesc clientOutBufferDesc;

		SecBuffer     serverOutBuffer[8192];
		SecBufferDesc serverOutBufferDesc[8192];

		///////////////////////////////////////////
		// Get the client and server credentials //
		///////////////////////////////////////////

		CredHandle clientCredentials;
		CredHandle serverCredentials;

		SECURITY_STATUS status;

		/*
			Acquires a HANDLE to the credentials for the given user
		*/
		//char ktxt[] = "Kerberos";
		//wchar_t wtext[20];
		//mbstowcs(wtext, ktxt, strlen(ktxt)+1);//Plus null
		//LPWSTR ptr = wtext;
		status = ::AcquireCredentialsHandle( NULL,
											 "Kerberos",
											 SECPKG_CRED_OUTBOUND,
											 NULL,
											 &auth,
											 NULL,
											 NULL,
											 &clientCredentials,
											 NULL );

		if(status != SEC_E_OK)
			return status;

		//////////////////////////////////////
		// Initialize the security contexts //
		//////////////////////////////////////

		CtxtHandle clientContext = {};
		unsigned long clientContextAttr = 0;

		CtxtHandle serverContext = {};
		unsigned long serverContextAttr = 0;

		/////////////////////////////
		// Clear the client buffer //
		/////////////////////////////

		clientOutBuffer.BufferType = SECBUFFER_TOKEN;
		clientOutBuffer.cbBuffer   = sizeof clientOutBufferData;
		clientOutBuffer.pvBuffer   = clientOutBufferData;

		clientOutBufferDesc.cBuffers  = 1;
		clientOutBufferDesc.pBuffers  = &clientOutBuffer;
		clientOutBufferDesc.ulVersion = SECBUFFER_VERSION;

		///////////////////////////////////
		// Initialize the client context //
		///////////////////////////////////
		//const size_t cSize = strlen(t)+1;
		//wchar_t* wc = new wchar_t[cSize];
		//mbstowcs (wc, t, cSize);

		status = InitializeSecurityContext( &clientCredentials,
											NULL,
											t,// the (service/domain) spn target that will authenticate this user "krbtgt/ad.utah.edu",
											0,
											0,
											SECURITY_NATIVE_DREP,
											NULL,
											0,
											&clientContext,
											&clientOutBufferDesc,
											&clientContextAttr,
											NULL );

		return status;
	}
}