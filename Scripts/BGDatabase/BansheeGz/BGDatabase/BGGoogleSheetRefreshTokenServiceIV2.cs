namespace BansheeGz.BGDatabase;

public interface BGGoogleSheetRefreshTokenServiceIV2 : BGGoogleSheetRefreshTokenServiceI
{
	string GetUrl(int port);

	void Exchange(int port, string code, out string token, out string refreshToken);
}
