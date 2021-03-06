﻿using Notify.Client;
using Notify.Models;
using Notify.Models.Responses;
using Notify.Exceptions;
using Notify.IntegrationTests.Assertions;
using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Notify.IntegrationTests
{
	[TestFixture]
	public class NotificationIntegrationClientTests
	{
		private NotificationClient client;

		private String NOTIFY_API_URL = Environment.GetEnvironmentVariable("NOTIFY_API_URL");
		private String API_KEY = Environment.GetEnvironmentVariable("API_KEY");
		private String API_SENDING_KEY = Environment.GetEnvironmentVariable("API_SENDING_KEY");

		private String FUNCTIONAL_TEST_NUMBER = Environment.GetEnvironmentVariable("FUNCTIONAL_TEST_NUMBER");
		private String FUNCTIONAL_TEST_EMAIL = Environment.GetEnvironmentVariable("FUNCTIONAL_TEST_EMAIL");

		private String EMAIL_TEMPLATE_ID = Environment.GetEnvironmentVariable("EMAIL_TEMPLATE_ID");
		private String SMS_TEMPLATE_ID = Environment.GetEnvironmentVariable("SMS_TEMPLATE_ID");
		private String LETTER_TEMPLATE_ID = Environment.GetEnvironmentVariable("LETTER_TEMPLATE_ID");
		private String EMAIL_REPLY_TO_ID = Environment.GetEnvironmentVariable("EMAIL_REPLY_TO_ID");
		private String SMS_SENDER_ID = Environment.GetEnvironmentVariable("SMS_SENDER_ID");
		private String INBOUND_SMS_QUERY_KEY = Environment.GetEnvironmentVariable("INBOUND_SMS_QUERY_KEY");

		private String smsNotificationId;
		private String emailNotificationId;
		private String letterNotificationId;

		const String TEST_TEMPLATE_SMS_BODY = "Hello ((name))\r\n\r\nFunctional Tests make our world a better place";
		const String TEST_SMS_BODY = "Hello someone\r\n\r\nFunctional Tests make our world a better place";

		const String TEST_TEMPLATE_EMAIL_BODY = "Hello ((name))\r\n\r\nFunctional test help make our world a better place";
		const String TEST_EMAIL_BODY = "Hello someone\r\n\r\nFunctional test help make our world a better place";
		const String TEST_EMAIL_SUBJECT = "Functional Tests are good";

		const String TEST_LETTER_BODY = "Hello Foo";
		const String TEST_LETTER_SUBJECT = "Main heading";

		[SetUp]
		[Test, Category("Integration")]
		public void SetUp()
		{
			this.client = new NotificationClient(NOTIFY_API_URL, API_KEY);
		}

		[Test, Category("Integration")]
		public void SendSmsTestWithPersonalisation()
		{
			Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
			{
				{ "name", "someone" }
			};

			SmsNotificationResponse response =
				this.client.SendSms(FUNCTIONAL_TEST_NUMBER, SMS_TEMPLATE_ID, personalisation, "sample-test-ref");
			this.smsNotificationId = response.id;
			Assert.IsNotNull(response);
			Assert.AreEqual(response.content.body, TEST_SMS_BODY);

			Assert.IsNotNull(response.reference);
			Assert.AreEqual(response.reference, "sample-test-ref");
		}

		[Test, Category("Integration")]
		public void GetSMSNotificationWithIdReturnsNotification()
		{
			SendSmsTestWithPersonalisation();
			Notification notification = this.client.GetNotificationById(this.smsNotificationId);

			Assert.IsNotNull(notification);
			Assert.IsNotNull(notification.id);
			Assert.AreEqual(notification.id, this.smsNotificationId);

			Assert.IsNotNull(notification.body);
			Assert.AreEqual(notification.body, TEST_SMS_BODY);

			Assert.IsNotNull(notification.reference);
			Assert.AreEqual(notification.reference, "sample-test-ref");

			NotifyAssertions.AssertNotification(notification);
		}

		[Test, Category("Integration")]
		public void SendEmailTestWithPersonalisation()
		{
			Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
			{
				{ "name", "someone" }
			};

			EmailNotificationResponse response =
				this.client.SendEmail(FUNCTIONAL_TEST_EMAIL, EMAIL_TEMPLATE_ID, personalisation);
			this.emailNotificationId = response.id;

			Assert.IsNotNull(response);
			Assert.AreEqual(response.content.body, TEST_EMAIL_BODY);
			Assert.AreEqual(response.content.subject, TEST_EMAIL_SUBJECT);
		}

		[Test, Category("Integration")]
		public void GetEmailNotificationWithIdReturnsNotification()
		{
			SendEmailTestWithPersonalisation();
			Notification notification = this.client.GetNotificationById(this.emailNotificationId);
			Assert.IsNotNull(notification);
			Assert.IsNotNull(notification.id);
			Assert.AreEqual(notification.id, this.emailNotificationId);

			Assert.IsNotNull(notification.body);
			Assert.AreEqual(notification.body, TEST_EMAIL_BODY);
			Assert.IsNotNull(notification.subject);
			Assert.AreEqual(notification.subject, TEST_EMAIL_SUBJECT);

			NotifyAssertions.AssertNotification(notification);
		}

		[Test, Category("Integration")]
		public void SendLetterTestWithPersonalisation()
		{
			Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
			{
				{ "address_line_1", "Foo" },
				{ "address_line_2", "Bar" },
				{ "postcode", "Baz" }
			};

			LetterNotificationResponse response =
				this.client.SendLetter(LETTER_TEMPLATE_ID, personalisation);

			this.letterNotificationId = response.id;

			Assert.IsNotNull(response);

			Assert.AreEqual(response.content.body, TEST_LETTER_BODY);
			Assert.AreEqual(response.content.subject, TEST_LETTER_SUBJECT);

		}

		[Test, Category("Integration")]
		public void GetLetterNotificationWithIdReturnsNotification()
		{
			SendLetterTestWithPersonalisation();
			Notification notification = this.client.GetNotificationById(this.letterNotificationId);

			Assert.IsNotNull(notification);
			Assert.IsNotNull(notification.id);
			Assert.AreEqual(notification.id, this.letterNotificationId);

			Assert.IsNotNull(notification.body);
			Assert.AreEqual(notification.body, TEST_LETTER_BODY);

			Assert.IsNotNull(notification.subject);
			Assert.AreEqual(notification.subject, TEST_LETTER_SUBJECT);

			NotifyAssertions.AssertNotification(notification);
		}

		[Test, Category("Integration")]
		public void GetAllNotifications()
		{
			NotificationList notificationsResponse = this.client.GetNotifications();
			Assert.IsNotNull(notificationsResponse);
			Assert.IsNotNull(notificationsResponse.notifications);

			List<Notification> notifications = notificationsResponse.notifications;

			foreach (Notification notification in notifications)
			{
				NotifyAssertions.AssertNotification(notification);
			}

		}

		[Test, Category("Integration")]
		public void GetReceivedTexts()
		{
			NotificationClient client_inbound = new NotificationClient(NOTIFY_API_URL, INBOUND_SMS_QUERY_KEY);
			ReceivedTextListResponse receivedTextListResponse = client_inbound.GetReceivedTexts();
			Assert.IsNotNull(receivedTextListResponse);
			Assert.IsNotNull(receivedTextListResponse.receivedTexts);
			Assert.IsTrue(receivedTextListResponse.receivedTexts.Count > 0);

			List<ReceivedTextResponse> receivedTexts = receivedTextListResponse.receivedTexts;

			foreach (ReceivedTextResponse receivedText in receivedTexts)
			{
				NotifyAssertions.AssertReceivedTextResponse(receivedText);
			}
		}

		[Test, Category("Integration")]
		public void GetNotificationWithInvalidIdRaisesClientException()
		{
			var ex = Assert.Throws<NotifyClientException>(() =>
				this.client.GetNotificationById("fa5f0a6e-5293-49f1-b99f-3fade784382f")
			);
			Assert.That(ex.Message, Does.Contain("No result found"));
		}

		[Test, Category("Integration")]
		public void GetTemplateWithInvalidIdRaisesClientException()
		{
			var ex = Assert.Throws<NotifyClientException>(() =>
				this.client.GetTemplateById("invalid_id")
			);
			Assert.That(ex.Message, Does.Contain("id is not a valid UUID"));
		}

		[Test, Category("Integration")]
		public void GetTemplateWithIdWithoutResultRaisesClientException()
		{
			var ex = Assert.Throws<NotifyClientException>(() =>
				this.client.GetTemplateById("fa5f0a6e-5293-49f1-b99f-3fade784382f")
			);
			Assert.That(ex.Message, Does.Contain("No result found"));
		}

		[Test, Category("Integration")]
		public void GetAllTemplates()
		{
			TemplateList templateList = this.client.GetAllTemplates();
			Assert.IsNotNull(templateList);
			Assert.IsTrue(templateList.templates.Count > 0);

			foreach (TemplateResponse template in templateList.templates)
			{
				NotifyAssertions.AssertTemplateResponse(template);
			}
		}

		[Test, Category("Integration")]
		public void GetAllSMSTemplates()
		{
			const String type = "sms";
			TemplateList templateList = this.client.GetAllTemplates(type);
			Assert.IsNotNull(templateList);
			Assert.IsTrue(templateList.templates.Count > 0);

			foreach (TemplateResponse template in templateList.templates)
			{
				NotifyAssertions.AssertTemplateResponse(template, type);
			}
		}

		[Test, Category("Integration")]
		public void GetAllEmailTemplates()
		{
			const String type = "email";
			TemplateList templateList = this.client.GetAllTemplates(type);
			Assert.IsNotNull(templateList);
			Assert.IsTrue(templateList.templates.Count > 0);

			foreach (TemplateResponse template in templateList.templates)
			{
				NotifyAssertions.AssertTemplateResponse(template, type);
			}
		}

		[Test, Category("Integration")]
		public void GetAllInvalidTemplatesRaisesClientException()
		{
			const String type = "invalid";

			var ex = Assert.Throws<NotifyClientException>(() => this.client.GetAllTemplates(type));
			Assert.That(ex.Message, Does.Contain("type invalid is not one of [sms, email, letter]"));
		}

		[Test, Category("Integration")]
		public void GetSMSTemplateWithId()
		{
			TemplateResponse template = this.client.GetTemplateById(SMS_TEMPLATE_ID);
			Assert.AreEqual(template.id, SMS_TEMPLATE_ID);
			Assert.AreEqual(template.body, TEST_TEMPLATE_SMS_BODY);
		}

		[Test, Category("Integration")]
		public void GetEmailTemplateWithId()
		{
			TemplateResponse template = this.client.GetTemplateById(EMAIL_TEMPLATE_ID);
			Assert.AreEqual(template.id, EMAIL_TEMPLATE_ID);
			Assert.AreEqual(template.body, TEST_TEMPLATE_EMAIL_BODY);
		}

		[Test, Category("Integration")]
		public void GenerateSMSPreviewWithPersonalisation()
		{
			Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
			{
				{ "name", "someone" }
			};

			TemplatePreviewResponse response =
				this.client.GenerateTemplatePreview(SMS_TEMPLATE_ID, personalisation);

			Assert.IsNotNull(response);
			Assert.AreEqual(response.body, TEST_SMS_BODY);
			Assert.AreEqual(response.subject, null);
		}

		[Test, Category("Integration")]
		public void GenerateEmailPreviewWithPersonalisation()
		{
			Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
			{
				{ "name", "someone" }
			};

			TemplatePreviewResponse response =
				this.client.GenerateTemplatePreview(EMAIL_TEMPLATE_ID, personalisation);

			Assert.IsNotNull(response);
			Assert.AreEqual(response.body, TEST_EMAIL_BODY);
			Assert.AreEqual(response.subject, TEST_EMAIL_SUBJECT);
		}

		[Test, Category("Integration")]
		public void GenerateEmailPreviewWithMissingPersonalisationRaisesClientException()
		{
			Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
			{
				{ "invalid", "personalisation" }
			};

			var ex = Assert.Throws<NotifyClientException>(() =>
				this.client.GenerateTemplatePreview(EMAIL_TEMPLATE_ID, personalisation)
			);
			Assert.That(ex.Message, Does.Contain("Missing personalisation: name"));
		}

		[Test, Category("Integration")]
		public void SendEmailTestServiceDefaultEmailReplyTo()
		{
			Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
			{
				{ "name", "someone" }
			};
			
			EmailNotificationResponse response = this.client.SendEmail(FUNCTIONAL_TEST_EMAIL, EMAIL_TEMPLATE_ID, personalisation);
			this.emailNotificationId = response.id;
			Assert.IsNotNull(response);
			Assert.AreEqual(response.content.body, TEST_EMAIL_BODY);
			Assert.AreEqual(response.content.subject, TEST_EMAIL_SUBJECT);
		}
		
		[Test, Category("Integration")]
		public void SendEmailTestSpecificEmailReplyTo()
		{
			Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
			{    
				{ "name", "someone" }
			};
			
			EmailNotificationResponse response = this.client.SendEmail(FUNCTIONAL_TEST_EMAIL, EMAIL_TEMPLATE_ID, personalisation, emailReplyToId: EMAIL_REPLY_TO_ID);
			this.emailNotificationId = response.id;
			Assert.IsNotNull(response);
			Assert.AreEqual(response.content.body, TEST_EMAIL_BODY);
			Assert.AreEqual(response.content.subject, TEST_EMAIL_SUBJECT);
		}
		
		[Test, Category("Integration")]
		public void SendEmailTestEmailReplyToNotPresent()
		{
			String fakeReplayToId = System.Guid.NewGuid().ToString();
			Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
			{    
				{ "name", "someone" }
			};
			
			var ex = Assert.Throws<NotifyClientException>(() => this.client.SendEmail(FUNCTIONAL_TEST_EMAIL, EMAIL_TEMPLATE_ID, personalisation, emailReplyToId: fakeReplayToId));
			Assert.That(ex.Message, Does.Contain("email_reply_to_id " + fakeReplayToId));
		}

		[Test, Category("Integration")]
		public void SendEmailTestAllArguments()
		{
			Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
			{    
				{ "name", "someone" }
			};
			
			EmailNotificationResponse response = this.client.SendEmail(FUNCTIONAL_TEST_EMAIL, EMAIL_TEMPLATE_ID, personalisation, clientReference: "TestReference", emailReplyToId: EMAIL_REPLY_TO_ID);
			this.emailNotificationId = response.id;
			Assert.IsNotNull(response);
			Assert.AreEqual(response.content.body, TEST_EMAIL_BODY);
			Assert.AreEqual(response.content.subject, TEST_EMAIL_SUBJECT);
		}

		[Test, Category("Integration")]
		public void SendSmsTestWithPersonalisationAndSmsSenderId()
		{
			Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
			{
				{ "name", "someone" }
			};

			NotificationClient client_sending = new NotificationClient(NOTIFY_API_URL, API_SENDING_KEY);

			SmsNotificationResponse response =
				client_sending.SendSms(FUNCTIONAL_TEST_NUMBER, SMS_TEMPLATE_ID, personalisation, "sample-test-ref", SMS_SENDER_ID);
			this.smsNotificationId = response.id;
			Assert.IsNotNull(response);
			Assert.AreEqual(response.content.body, TEST_SMS_BODY);

			Assert.IsNotNull(response.reference);
			Assert.AreEqual(response.reference, "sample-test-ref");
		}


	}
}
