using AngleSharp.Dom;
using AngleSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Pamello51
{
	public class YoutubeHttpInfo {
		public string Title;
		public string? Episodes;
	}

	public static class YoutubeHttpParser
	{
		public static async Task<YoutubeHttpInfo> Parce(string url) {
			YoutubeHttpInfo info = new YoutubeHttpInfo();
			info.Episodes = null;

			HttpClient client = new HttpClient();
			HttpResponseMessage response = client.Send(new HttpRequestMessage(new HttpMethod("get"), url));

			Stream responceStream = await response.Content.ReadAsStreamAsync();

			IConfiguration config = Configuration.Default.WithDefaultLoader();
			IBrowsingContext context = BrowsingContext.New(config);
			IDocument document = await context.OpenAsync(req => req.Content(responceStream));

			IHtmlCollection<IElement> metaElements = document.QuerySelectorAll("meta");
			foreach (IElement metaElement in metaElements) {
				if (metaElement.GetAttribute("name") == "title") {
					info.Title = metaElement.GetAttribute("content") ?? "No Title";
					break;
				}
			}

			string? jsonStr = null;

			IHtmlCollection<IElement> scriptElements = document.QuerySelectorAll("script");
			foreach (IElement scriptElement in scriptElements) {
				if (scriptElement.InnerHtml.Length > 100000) {
					jsonStr = scriptElement.InnerHtml.Substring(20);
				}
			}
			jsonStr = jsonStr?.Remove(jsonStr.Length - 1, 1);

			if (jsonStr is null) {
				return info;
			}

			JsonElement? chapterRendererElements = null;
			try {
				JsonDocument json = JsonDocument.Parse(jsonStr ?? "{}");
				chapterRendererElements = json.RootElement.GetProperty("playerOverlays")
					.GetProperty("playerOverlayRenderer")
					.GetProperty("decoratedPlayerBarRenderer")
					.GetProperty("decoratedPlayerBarRenderer")
					.GetProperty("playerBar")
					.GetProperty("multiMarkersPlayerBarRenderer")
					.GetProperty("markersMap")
					[0]
					.GetProperty("value")
					.GetProperty("chapters");
			}
			catch {
				return info;
			}
			if (chapterRendererElements is null) {
				return info;
			}

			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < chapterRendererElements?.GetArrayLength(); i++) {
				sb.Append($"{chapterRendererElements?[i]
					.GetProperty("chapterRenderer")
					.GetProperty("title")
					.GetProperty("simpleText")}");
				sb.Append(':');
				sb.Append($"{chapterRendererElements?[i]
					.GetProperty("chapterRenderer")
					.GetProperty("timeRangeStartMillis")}");
				sb.Append(';');
			}

			info.Episodes = sb.ToString();

			return info;
		}
	}
}
