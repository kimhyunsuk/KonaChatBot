﻿namespace KonaChatBot.Dialogs

{
	using System;
	using System.Net.Http;
	using System.Threading.Tasks;
	using Microsoft.Bot.Builder.Dialogs;
	using Microsoft.Bot.Connector;
	using System.Diagnostics;
	using System.Net;
	using System.IO;
	using System.Text;
	using Newtonsoft.Json;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
	using System.Configuration;
	using System.Web.Configuration;
	using KonaChatBot.DB;
	using KonaChatBot;
	using KonaChatBot.Models;

	[Serializable]
	public class IntentNoneDialog : IDialog<object>
	{

		//public static int sorryMessageCnt = 0;
		public static string newUserID = "";
		public static string beforeUserID = "";
		private string luis_intent;
		private string entitiesStr;
		private DateTime startTime;
		public static string beforeMessgaeText = "";
		public static string messgaeText = "";
		private string orgKRMent;
		private string orgENGMent;

		public static Configuration rootWebConfig = WebConfigurationManager.OpenWebConfiguration("/testkonabot");
		//const string redirectEventPageURL = "redirectPageURL";
		//string eventURL = rootWebConfig.ConnectionStrings.ConnectionStrings[redirectEventPageURL].ToString();

		public IntentNoneDialog(string luis_intent, string entitiesStr, DateTime startTime, string orgKRMent, string orgENGMent)
		{
			this.luis_intent = luis_intent;
			this.entitiesStr = entitiesStr;
			this.startTime = startTime;
			this.orgKRMent = orgKRMent;
			this.orgENGMent = orgENGMent;

		}

		public async Task StartAsync(IDialogContext context)
		{
			/* Wait until the first message is received from the conversation and call MessageReceviedAsync 
             *  to process that message. */

			context.Wait(this.MessageReceivedAsync);
		}

		private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
		{
			/* When MessageReceivedAsync is called, it's passed an IAwaitable<IMessageActivity>. To get the message,
             *  await the result. */

			Debug.WriteLine("luis_intent : " + luis_intent);
			Debug.WriteLine("entitiesStr : " + entitiesStr);

			// Db
			DbConnect db = new DbConnect();

			Debug.WriteLine("activity : " + context.Activity.Conversation.Id);

			newUserID = context.Activity.Conversation.Id;
			if (beforeUserID != newUserID)
			{
				beforeUserID = newUserID;
				MessagesController.sorryMessageCnt = 0;
			}

			var message = await result;
			beforeMessgaeText = message.Text;
			if (message.Text.Contains("코나") == true)
			{
				messgaeText = message.Text;
				if (messgaeText.Contains("현대자동차") != true || messgaeText.Contains("현대 자동차") != true)
				{
					messgaeText = "현대자동차 " + messgaeText;
				}
			}
			else
			{
				messgaeText = "코나 " + message.Text;
				if (messgaeText.Contains("현대자동차") != true || messgaeText.Contains("현대 자동차") != true)
				{
					messgaeText = "현대자동차 " + messgaeText;
				}
			}

			if (messgaeText.Contains("코나") == true && (messgaeText.Contains("현대자동차") == true || messgaeText.Contains("현대 자동차") == true))
			{
				var reply = context.MakeMessage();
				Debug.WriteLine("SERARCH MESSAGE : " + messgaeText);
				if ((messgaeText != null) && messgaeText.Trim().Length > 0)
				{
					//Naver Search API

					string url = "https://openapi.naver.com/v1/search/news.json?query=" + messgaeText + "&display=10&start=1&sort=sim"; //news JSON result 
					//string blogUrl = "https://openapi.naver.com/v1/search/blog.json?query=" + messgaeText + "&display=10&start=1&sort=sim"; //search JSON result 
					//string cafeUrl = "https://openapi.naver.com/v1/search/cafearticle.json?query=" + messgaeText + "&display=10&start=1&sort=sim"; //cafe JSON result 
					//string url = "https://openapi.naver.com/v1/search/blog.xml?query=" + query; //blog XML result
					HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
					request.Headers.Add("X-Naver-Client-Id", "Y536Z1ZMNv93Oej6TrkF");
					request.Headers.Add("X-Naver-Client-Secret", "cPHOFK6JYY");
					HttpWebResponse response = (HttpWebResponse)request.GetResponse();
					string status = response.StatusCode.ToString();
					if (status == "OK")
					{
						Stream stream = response.GetResponseStream();
						StreamReader reader = new StreamReader(stream, Encoding.UTF8);
						string text = reader.ReadToEnd();

						RootObject serarchList = JsonConvert.DeserializeObject<RootObject>(text);

						Debug.WriteLine("serarchList : " + serarchList);
						//description

						if (serarchList.display == 1)
						{
							Debug.WriteLine("SERARCH : " + Regex.Replace(serarchList.items[0].title, @"[^<:-:>-<b>-</b>]", "", RegexOptions.Singleline));

							if (serarchList.items[0].title.Contains("코나"))
							{
								//Only One item
								List<CardImage> cardImages = new List<CardImage>();
								CardImage img = new CardImage();
								img.Url = "";
								cardImages.Add(img);

								string searchTitle = "";
								string searchText = "";

								searchTitle = serarchList.items[0].title;
								searchText = serarchList.items[0].description;



								if (context.Activity.ChannelId == "facebook")
								{
									searchTitle = Regex.Replace(searchTitle, @"[<][a-z|A-Z|/](.|)*?[>]", "", RegexOptions.Singleline).Replace("\n", "").Replace("<:", "").Replace(":>", "");
									searchText = Regex.Replace(searchText, @"[<][a-z|A-Z|/](.|)*?[>]", "", RegexOptions.Singleline).Replace("\n", "").Replace("<:", "").Replace(":>", "");
								}


								LinkHeroCard card = new LinkHeroCard()
								{
									Title = searchTitle,
									Subtitle = null,
									Text = searchText,
									Images = cardImages,
									Buttons = null,
									Link = Regex.Replace(serarchList.items[0].link, "amp;", "")
								};
								var attachment = card.ToAttachment();

								reply.Attachments = new List<Attachment>();
								reply.Attachments.Add(attachment);
							}
						}
						else
						{
							reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
							reply.Attachments = new List<Attachment>();
							for (int i = 0; i < serarchList.display; i++)
							{
								string searchTitle = "";
								string searchText = "";

								searchTitle = serarchList.items[i].title;
								searchText = serarchList.items[i].description;

								if (context.Activity.ChannelId == "facebook")
								{
									searchTitle = Regex.Replace(searchTitle, @"[<][a-z|A-Z|/](.|)*?[>]", "", RegexOptions.Singleline).Replace("\n", "").Replace("<:", "").Replace(":>", "");
									searchText = Regex.Replace(searchText, @"[<][a-z|A-Z|/](.|)*?[>]", "", RegexOptions.Singleline).Replace("\n", "").Replace("<:", "").Replace(":>", "");
								}

								if (serarchList.items[i].title.Contains("코나"))
								{
									List<CardImage> cardImages = new List<CardImage>();
									CardImage img = new CardImage();
									img.Url = "";
									cardImages.Add(img);

									List<CardAction> cardButtons = new List<CardAction>();
									CardAction[] plButton = new CardAction[1];
									plButton[0] = new CardAction()
									{
										Value = Regex.Replace(serarchList.items[i].link, "amp;", ""),
										Type = "openUrl",
										Title = "기사 바로가기"
									};
									cardButtons = new List<CardAction>(plButton);

									if (context.Activity.ChannelId == "facebook")
									{
										LinkHeroCard card = new LinkHeroCard()
										{
											Title = searchTitle,
											Subtitle = null,
											Text = searchText,
											Images = cardImages,
											Buttons = cardButtons,
											Link = null
										};
										var attachment = card.ToAttachment();
										reply.Attachments.Add(attachment);
									}
									else
									{
										LinkHeroCard card = new LinkHeroCard()
										{
											Title = searchTitle,
											Subtitle = null,
											Text = searchText,
											Images = cardImages,
											Buttons = null,
											Link = Regex.Replace(serarchList.items[i].link, "amp;", "")
										};
										var attachment = card.ToAttachment();
										reply.Attachments.Add(attachment);
									}
								}
							}
						}
						await context.PostAsync(reply);



						if (reply.Attachments.Count == 0)
						{

							await this.SendSorryMessageAsync(context);

						}
						else
						{

							orgKRMent = Regex.Replace(message.Text, @"[^a-zA-Z0-9ㄱ-힣]", "", RegexOptions.Singleline);


							for (int n = 0; n < Regex.Split(message.Text, " ").Length; n++)
							{
								string chgMsg = db.SelectChgMsg(Regex.Split(message.Text, " ")[n]);
								if (!string.IsNullOrEmpty(chgMsg))
								{
									message.Text = message.Text.Replace(Regex.Split(message.Text, " ")[n], chgMsg);
								}
							}


							Translator translateInfo = await getTranslate(message.Text);

							orgENGMent = Regex.Replace(translateInfo.data.translations[0].translatedText, @"[^a-zA-Z0-9ㄱ-힣-\s-&#39;]", "", RegexOptions.Singleline);

							orgENGMent = orgENGMent.Replace("&#39;", "'");

							//int dbResult = db.insertUserQuery(orgKRMent, orgENGMent, "", "", "", 1, 'S', "", "", "", "SEARCH", MessagesController.userData.GetProperty<int>("appID"));
							int dbResult = db.insertUserQuery(orgKRMent,  "", "", "", "", 'S', MessagesController.chatBotID);
							Debug.WriteLine("INSERT QUERY RESULT : " + dbResult.ToString());

							DateTime endTime = DateTime.Now;

							Debug.WriteLine("USER NUMBER : " + context.Activity.Conversation.Id);
							Debug.WriteLine("CUSTOMMER COMMENT KOREAN : " + messgaeText.Replace("코나 ", ""));
							Debug.WriteLine("CUSTOMMER COMMENT ENGLISH : " + translateInfo.data.translations[0].translatedText.Replace("&#39;", "'"));
							Debug.WriteLine("CHANNEL_ID : " + context.Activity.ChannelId);
							Debug.WriteLine("프로그램 수행시간 : {0}/ms", ((endTime - startTime).Milliseconds));

							//int inserResult = db.insertHistory(context.Activity.Conversation.Id, messgaeText.Replace("코나 ", ""), translateInfo.data.translations[0].translatedText.Replace("&#39;", "'"), "SEARCH", context.Activity.ChannelId, ((endTime - startTime).Milliseconds), MessagesController.userData.GetProperty<int>("appID"));
							int inserResult = db.insertHistory(context.Activity.Conversation.Id, messgaeText, "SEARCH", context.Activity.ChannelId, ((endTime - startTime).Milliseconds), MessagesController.chatBotID);
							if (inserResult > 0)
							{
								Debug.WriteLine("HISTORY RESULT SUCCESS");
							}
							else
							{
								Debug.WriteLine("HISTORY RESULT FAIL");
							}
							HistoryLog("[ SEARCH ] ==>> userID :: [ " + context.Activity.Conversation.Id + " ]       message :: [ " + messgaeText.Replace("코나 ", "") + " ]       date :: [ " + DateTime.Now + " ]");
						}

					}
					else
					{
						System.Diagnostics.Debug.WriteLine("Error 발생=" + status);
						await this.SendSorryMessageAsync(context);
					}

					context.Done(messgaeText);

				}
			}
			else
			{
				await this.SendSorryMessageAsync(context);
			}
		}

		private static async Task<Translator> getTranslate(string input)
		{
			Translator trans = new Translator();

			using (HttpClient client = new HttpClient())
			{
				string appId = "AIzaSyDr4CH9BVfENdM9uoSK0fANFVWD0gGXlJM";

				string url = string.Format("https://translation.googleapis.com/language/translate/v2/?key={0}&q={1}&source=ko&target=en&model=nmt", appId, input);

				HttpResponseMessage msg = await client.GetAsync(url);

				if (msg.IsSuccessStatusCode)
				{
					var JsonDataResponse = await msg.Content.ReadAsStringAsync();
					trans = JsonConvert.DeserializeObject<Translator>(JsonDataResponse);
				}
				return trans;
			}

		}


		private async Task SendSorryMessageAsync(IDialogContext context)
		{
			// Db
			DbConnect db = new DbConnect();

			Debug.WriteLine("root before sorry count : " + MessagesController.sorryMessageCnt);
			HistoryLog("root before sorry count : " + MessagesController.sorryMessageCnt);


			//int sorryMessageCheck = db.SelectUserQueryErrorMessageCheck(context.Activity.Conversation.Id, MessagesController.userData.GetProperty<int>("appID"));
			int sorryMessageCheck = db.SelectUserQueryErrorMessageCheck(context.Activity.Conversation.Id, MessagesController.chatBotID);

			++MessagesController.sorryMessageCnt;

			var reply_err = context.MakeMessage();
			reply_err.Recipient = context.Activity.From;
			reply_err.Type = "message";
            reply_err.Attachments = new List<Attachment>();
            reply_err.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            Debug.WriteLine("root after sorry count : " + MessagesController.sorryMessageCnt);
			HistoryLog("root after sorry count : " + MessagesController.sorryMessageCnt);
            List<TextList> text = new List<TextList>();
            if (sorryMessageCheck == 0)
            {
                text = db.SelectSorryDialogText("5");
            }
            else
            {
                text = db.SelectSorryDialogText("6");
            }
            
            for (int i = 0; i < text.Count; i++)
            {
                HeroCard plCard = new HeroCard()
                {
                    Title = text[i].cardTitle,
                    Text = text[i].cardText
                };

                Attachment plAttachment = plCard.ToAttachment();
                reply_err.Attachments.Add(plAttachment);
            }

            //reply_err.Text = SorryMessageList.GetSorryMessage(sorryMessageCheck);
			await context.PostAsync(reply_err);

			orgKRMent = Regex.Replace(beforeMessgaeText, @"[^a-zA-Z0-9ㄱ-힣]", "", RegexOptions.Singleline);


			for (int n = 0; n < Regex.Split(beforeMessgaeText, " ").Length; n++)
			{
				string chgMsg = db.SelectChgMsg(Regex.Split(beforeMessgaeText, " ")[n]);
				if (!string.IsNullOrEmpty(chgMsg))
				{
					beforeMessgaeText = beforeMessgaeText.Replace(Regex.Split(beforeMessgaeText, " ")[n], chgMsg);
				}
			}

            //Translator translateInfo = await getTranslate(beforeMessgaeText);

            //orgENGMent = Regex.Replace(translateInfo.data.translations[0].translatedText, @"[^a-zA-Z0-9ㄱ-힣-\s-&#39;]", "", RegexOptions.Singleline);

            //orgENGMent = orgENGMent.Replace("&#39;", "'");

            //int dbResult = db.insertUserQuery(orgKRMent, orgENGMent, "", "", "", 0, 'D', "", "", "", "SEARCH", MessagesController.userData.GetProperty<int>("appID"));
            int dbResult = db.insertUserQuery(orgKRMent,  "", "", "", "", 'D',  MessagesController.chatBotID);
			Debug.WriteLine("INSERT QUERY RESULT : " + dbResult.ToString());


			DateTime endTime = DateTime.Now;

			Debug.WriteLine("USER NUMBER : " + context.Activity.Conversation.Id);
			Debug.WriteLine("CUSTOMMER COMMENT KOREAN : " + beforeMessgaeText);
			//Debug.WriteLine("CUSTOMMER COMMENT ENGLISH : " + translateInfo.data.translations[0].translatedText.Replace("&#39;", "'"));
			Debug.WriteLine("CHANNEL_ID : " + context.Activity.ChannelId);
			Debug.WriteLine("프로그램 수행시간 : {0}/ms", ((endTime - startTime).Milliseconds));

			//int inserResult = db.insertHistory(context.Activity.Conversation.Id, beforeMessgaeText, translateInfo.data.translations[0].translatedText.Replace("&#39;", "'"), "SEARCH", context.Activity.ChannelId, ((endTime - startTime).Milliseconds), MessagesController.userData.GetProperty<int>("appID"));
			int inserResult = db.insertHistory(context.Activity.Conversation.Id, beforeMessgaeText,  "ERROR", context.Activity.ChannelId, ((endTime - startTime).Milliseconds), MessagesController.chatBotID);
			if (inserResult > 0)
			{
				Debug.WriteLine("HISTORY RESULT SUCCESS");
			}
			else
			{
				Debug.WriteLine("HISTORY RESULT FAIL");
			}
		}

		public void HistoryLog(String strMsg)
		{
			try
			{

				string m_strLogPrefix = AppDomain.CurrentDomain.BaseDirectory + @"LOG\";
				string m_strLogExt = @".LOG";
				DateTime dtNow = DateTime.Now;
				string strDate = dtNow.ToString("yyyy-MM-dd");
				string strPath = String.Format("{0}{1}{2}", m_strLogPrefix, strDate, m_strLogExt);
				string strDir = Path.GetDirectoryName(strPath);
				DirectoryInfo diDir = new DirectoryInfo(strDir);

				if (!diDir.Exists)
				{
					diDir.Create();
					diDir = new DirectoryInfo(strDir);
				}

				if (diDir.Exists)
				{
					System.IO.StreamWriter swStream = File.AppendText(strPath);
					string strLog = String.Format("{0}: {1}", dtNow.ToString(dtNow.Hour + "시mm분ss초"), strMsg);
					swStream.WriteLine(strLog);
					swStream.Close(); ;
				}
			}
			catch (System.Exception e)
			{
				Debug.WriteLine(e.Message);
			}
		}
	}
}