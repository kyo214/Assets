namespace BansheeGz.BGDatabase;

public interface BGGoogleSheetRefreshTokenServiceI
{
	string Url { get; }

	void Exchange(string code, out string token, out string refreshToken);
}
