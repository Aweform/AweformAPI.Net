using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;

namespace Aweform {

	//
	// AweformAPI.Net
	// AweformAPI.Net depends on AweformJSON.Net which you can get from GitHub at
	// https://github.com/Aweform/AweformJSON.Net
	////////////////////////////////////////////////////////////////////////////////////

		public class AweformException : Exception {

			public AweformException() : base() { }
			public AweformException(String message) : base(message) { }
			public AweformException(String message, Exception inner) : base(message, inner) { }
		}

		public class AweformAPI {

			private String APIKey = "";
			private String APIURL = "";

			public AweformAPI(String apiKey, String apiURL = "https://aweform.com/api") {

				if (String.IsNullOrEmpty(apiKey)) { throw new AweformException("You must provide an API key"); }
				if (String.IsNullOrEmpty(apiURL)) { throw new AweformException("You must provide the API URL"); }

				APIKey = apiKey;
				APIURL = apiURL;
			}

			public AweformUser GetMe() {

				return AweformUserFromJSON(Request("/me/"));
			}

			public AweformResponse GetResponse(Int64 responseId) {

				return AweformResponseFromJSON(Request("/response/" + responseId + "/"));
			}

			public List<AweformResponse> GetResponsesForForm(Int64 formId, Int32 from = 0, Int32 count = 100) {

				return AweformResponsesFromJSON(RequestPaged("/form/" + formId + "/responses/", from, count));
			}

			public List<AweformResponse> GetResponsesInWorkspace(Int64 workspaceId, Int32 from = 0, Int32 count = 100) {

				return AweformResponsesFromJSON(RequestPaged("/workspace/" + workspaceId + "/responses/", from, count));
			}

			public List<AweformResponse> GetResponses(Int32 from = 0, Int32 count = 100) {

				return AweformResponsesFromJSON(RequestPaged("/responses/", from, count));
			}

			public AweformFormDefinition GetFormDefinitionForForm(Int64 formId) {

				return AweformFormDefinitionFromJSON(Request("/form/" + formId + "/formDefinition/"));
			}

			public List<AweformFormDefinition> GetFormDefinitionsInWorkspace(Int64 workspaceId, Int32 from = 0, Int32 count = 100) {

				return AweformFormDefinitionsFromJSON(RequestPaged("/workspace/" + workspaceId + "/formDefinitions/", from, count));
			}

			public List<AweformFormDefinition> GetFormDefinitions(Int32 from = 0, Int32 count = 100) {

				return AweformFormDefinitionsFromJSON(RequestPaged("/formDefinitions/", from, count));
			}

			public AweformForm GetForm(Int64 formId) {

				return AweformFormFromJSON(Request("/form/" + formId + "/"));
			}

			public List<AweformForm> GetFormsInWorkspace(Int64 workspaceId, Int32 from = 0, Int32 count = 100) {

				return AweformFormsFromJSON(RequestPaged("/workspace/" + workspaceId + "/forms/", from, count));
			}

			public List<AweformForm> GetForms(Int32 from = 0, Int32 count = 100) {

				return AweformFormsFromJSON(RequestPaged("/forms/", from, count));
			}

			public AweformWorkspace GetWorkspace(Int64 workspaceId) {

				return AweformWorkspaceFromJSON(Request("/workspace/" + workspaceId + "/"));
			}

			public List<AweformWorkspace> GetWorkspaces(Int32 from = 0, Int32 count = 100) {

				return AweformWorkspacesFromJSON(RequestPaged("/workspaces/", from, count));
			}

			private static AweformUser AweformUserFromJSON(AweformJSON.Element json) {

				AweformUser user = new AweformUser();
				user.Id = json.GetAttributeAsInt64("id");
				user.EmailAddress = json.GetAttributeAsString("emailAddress");
				user.Name = json.GetAttributeAsString("name");
				user.AccountType = json.GetAttributeAsString("accountType");

				return user;
			}

			private static List<AweformForm> AweformFormsFromJSON(AweformJSON.Element json) {

				List<AweformForm> forms = new List<AweformForm>();

				foreach (AweformJSON.Element jsonForm in json.Elements) {

					forms.Add(AweformFormFromJSON(jsonForm));
				}

				return forms;
			}

			private static AweformForm AweformFormFromJSON(AweformJSON.Element json) {
			
				AweformForm form = new AweformForm();
				form.Id = json.GetAttributeAsInt64("id");
				form.Name = json.GetAttributeAsString("name");
				form.WorkspaceId = json.GetAttributeAsInt64("workspaceId");

				return form;
			}

			private static List<AweformWorkspace> AweformWorkspacesFromJSON(AweformJSON.Element json) {

				List<AweformWorkspace> workspaces = new List<AweformWorkspace>();

				foreach (AweformJSON.Element jsonForm in json.Elements) {

					workspaces.Add(AweformWorkspaceFromJSON(jsonForm));
				}

				return workspaces;
			}

			private static AweformWorkspace AweformWorkspaceFromJSON(AweformJSON.Element json) {
			
				AweformWorkspace workspace = new AweformWorkspace();
				workspace.Id = json.GetAttributeAsInt64("id");
				workspace.Name = json.GetAttributeAsString("name");

				return workspace;
			}

			private static List<AweformResponse> AweformResponsesFromJSON(AweformJSON.Element json) {

				List<AweformResponse> responses = new List<AweformResponse>();

				foreach (AweformJSON.Element jsonForm in json.Elements) {

					responses.Add(AweformResponseFromJSON(jsonForm));
				}

				return responses;
			}

			private static AweformResponse AweformResponseFromJSON(AweformJSON.Element json) {
			
				AweformResponse response = new AweformResponse();
				response.Id = json.GetAttributeAsInt64("id");
				response.DateInUtc = DateTime.Parse(json.GetAttributeAsString("dateInUTC"), CultureInfo.InvariantCulture);
				response.FormId = json.GetAttributeAsInt64("formId");
				response.WorkspaceId = json.GetAttributeAsInt64("workspaceId");

				List<AweformJSON.Element> attributes = json.GetAttributes();

				foreach (AweformJSON.Element attribute in attributes) {

					if (attribute.Name == "id" || attribute.Name == "dateInUTC" || attribute.Name == "formId" || attribute.Name == "workspaceId") {

						continue;
					}

					AweformQuestionAndAnswer questionAndAnswer = new AweformQuestionAndAnswer();
					questionAndAnswer.Question = attribute.Name;
					questionAndAnswer.Answer = attribute.Value;

					response.Answers.Add(questionAndAnswer);
				}

				return response;
			}

			private static List<AweformFormDefinition> AweformFormDefinitionsFromJSON(AweformJSON.Element json) {

				List<AweformFormDefinition> formsDefinitions = new List<AweformFormDefinition>();

				foreach (AweformJSON.Element jsonForm in json.Elements) {

					formsDefinitions.Add(AweformFormDefinitionFromJSON(jsonForm));
				}

				return formsDefinitions;
			}

			private static AweformFormDefinition AweformFormDefinitionFromJSON(AweformJSON.Element json) {
			
				AweformFormDefinition formDefinition = new AweformFormDefinition();
				formDefinition.Id = json.GetAttributeAsInt64("id");
				formDefinition.Name = json.GetAttributeAsString("name");
				formDefinition.WorkspaceId = json.GetAttributeAsInt64("workspaceId");

				AweformJSON.Element components = json.GetAttribute("components");

				foreach (AweformJSON.Element component in components.Elements) {

					AweformFormComponent formComponent = new AweformFormComponent();
					formComponent.Name = component.GetAttributeAsString("name");
					formComponent.Type = (AweformFormComponentType)Enum.Parse(typeof(AweformFormComponentType), component.GetAttributeAsString("type"));

					if (formComponent.Type == AweformFormComponentType.PictureChoice || formComponent.Type == AweformFormComponentType.Rating || formComponent.Type == AweformFormComponentType.Select || formComponent.Type == AweformFormComponentType.YesNo) {

						formComponent.Options = new List<String>();

						AweformJSON.Element options = component.GetAttribute("options");

						foreach (AweformJSON.Element option in options.Elements) {

							formComponent.Options.Add(option.Value);
						}
					}

					formDefinition.Components.Add(formComponent);
				}

				return formDefinition;
			}

			private AweformJSON.Element RequestPaged(String url, Int32 from, Int32 count) {

				Int32 startPage = (from / 100);
				Int32 endPage = ((from + count - 1) / 100);

				Int32 startRequestIndex = (startPage * 100);

				Int32 page = startPage;

				AweformJSON.Element paged = null;

				while (page <= endPage) {

					AweformJSON.Element singlePage = Request(url + "?page=" + page);

					if (paged == null) {

						paged = singlePage;

					} else {

						foreach (AweformJSON.Element component in singlePage.Elements) {

							paged.Elements.Add(component);
						}
					}

					if (singlePage.Elements.Count == 0) {

						break;
					}

					page += 1;
				}

				from -= startRequestIndex;

				if (from > 0) {

					paged.Elements.RemoveRange(0, from);
				}

				if (count < paged.Elements.Count) {

					paged.Elements.RemoveRange(count, paged.Elements.Count - count);
				}

				if (paged == null) {

					throw new AweformException("Zero pages returned");
				}

				return paged;		
			}

			protected AweformJSON.Element Request(String url) {

				WebClient wc = new WebClient();

				String response = wc.DownloadString(APIURL + url + (url.Contains("?", StringComparison.InvariantCulture) ? "&" : "?") + "apiKey=" + APIKey);
				wc.Dispose();

				AweformJSON.Element component = AweformJSON.Parse(response);

				if (component.GetAttribute("error") != null) {

					throw new AweformException(component.GetAttributeAsString("error"));
				}

				return component;
			}
		}

		public class AweformUser {

			public Int64 Id { get; set; }
			public String EmailAddress { get; set; }
			public String Name { get; set; }
			public String AccountType { get; set; }
		}

		public class AweformWorkspace {

			public Int64 Id { get; set; }
			public String Name { get; set; }
		}

		public class AweformForm {

			public Int64 Id { get; set; }
			public String Name { get; set; }
			public Int64 WorkspaceId { get; set; }
		}

		public class AweformResponse {

			public Int64 Id { get; set; }
			public DateTime DateInUtc { get; set; }
			public Int64 FormId { get; set; }
			public Int64 WorkspaceId { get; set; }
			public List<AweformQuestionAndAnswer> Answers { get; }
		
			public AweformResponse() {

				Answers = new List<AweformQuestionAndAnswer>();
			}
		}

		public class AweformQuestionAndAnswer {

			public String Question { get; set; }
			public String Answer { get; set; }
		}

		public class AweformFormDefinition {

			public Int64 Id { get; set; }
			public String Name { get; set; }
			public Int64 WorkspaceId { get; set; }
			public List<AweformFormComponent> Components { get; }

			public AweformFormDefinition() {
			 
				Components = new List<AweformFormComponent>();
			}
		}
				
		public enum AweformFormComponentType {

			HiddenValue,
			ShortText,
			LongText,
			Number,
			Select,
			Date,
			Email,
			Screen,
			Rating,
			EndScreen,
			PictureChoice,
			YesNo
		}

		public class AweformFormComponent {
				
			public AweformFormComponentType Type { get; set; }
			public String Name { get; set; }
			public List<String> Options { get; set; }

			public AweformFormComponent() {

				Options = null;
			}
		}
}
