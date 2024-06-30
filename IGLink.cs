public interface IGLink
{
	void executeHome();

	void executeArticle(int articleId);

	void executeArticlePost();

	void executeArticlePostWithImage(string filePath);

	void executeArticlePostWithVideo(string filePath);

	void syncGameUserId(string gameUserId);

	void startWidget();

	void stopWidget();

	void setUseWidgetVideoRecord(bool useVideoRecord);

	void setUseWidgetScreenShot(bool useScreenShot);

	void setShowWidgetWhenUnloadSDK(bool useWidget);

	void executeMore();

	string getCurrentChannelCode();

	void setChannelCode(string channelCode);

	void setThemeColor(string themeColorCSSString);

	void setThemeColor(string themeColorCSSString, string backgroundCSSString);

	void setWidgetStartPosition(bool isLeft, int heightPercentage);
}
