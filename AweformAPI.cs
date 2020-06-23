using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;

namespace Aweform {

	//
	// AweformAPI
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

			private static AweformUser AweformUserFromJSON(AweformJSON.Component json) {

				AweformUser user = new AweformUser();
				user.Id = json.GetAttributeAsInt64("id");
				user.EmailAddress = json.GetAttributeAsString("emailAddress");
				user.Name = json.GetAttributeAsString("name");
				user.AccountType = json.GetAttributeAsString("accountType");

				return user;
			}

			private static List<AweformForm> AweformFormsFromJSON(AweformJSON.Component json) {

				List<AweformForm> forms = new List<AweformForm>();

				foreach (AweformJSON.Component jsonForm in json.Values) {

					forms.Add(AweformFormFromJSON(jsonForm));
				}

				return forms;
			}

			private static AweformForm AweformFormFromJSON(AweformJSON.Component json) {
			
				AweformForm form = new AweformForm();
				form.Id = json.GetAttributeAsInt64("id");
				form.Name = json.GetAttributeAsString("name");
				form.WorkspaceId = json.GetAttributeAsInt64("workspaceId");

				return form;
			}

			private static List<AweformWorkspace> AweformWorkspacesFromJSON(AweformJSON.Component json) {

				List<AweformWorkspace> workspaces = new List<AweformWorkspace>();

				foreach (AweformJSON.Component jsonForm in json.Values) {

					workspaces.Add(AweformWorkspaceFromJSON(jsonForm));
				}

				return workspaces;
			}

			private static AweformWorkspace AweformWorkspaceFromJSON(AweformJSON.Component json) {
			
				AweformWorkspace workspace = new AweformWorkspace();
				workspace.Id = json.GetAttributeAsInt64("id");
				workspace.Name = json.GetAttributeAsString("name");

				return workspace;
			}

			private static List<AweformResponse> AweformResponsesFromJSON(AweformJSON.Component json) {

				List<AweformResponse> responses = new List<AweformResponse>();

				foreach (AweformJSON.Component jsonForm in json.Values) {

					responses.Add(AweformResponseFromJSON(jsonForm));
				}

				return responses;
			}

			private static AweformResponse AweformResponseFromJSON(AweformJSON.Component json) {
			
				AweformResponse response = new AweformResponse();
				response.Id = json.GetAttributeAsInt64("id");
				response.DateInUtc = DateTime.Parse(json.GetAttributeAsString("dateInUTC"), CultureInfo.InvariantCulture);
				response.FormId = json.GetAttributeAsInt64("formId");
				response.WorkspaceId = json.GetAttributeAsInt64("workspaceId");

				List<AweformJSON.Component> attributes = json.GetAttributes();

				foreach (AweformJSON.Component attribute in attributes) {

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

			private static List<AweformFormDefinition> AweformFormDefinitionsFromJSON(AweformJSON.Component json) {

				List<AweformFormDefinition> formsDefinitions = new List<AweformFormDefinition>();

				foreach (AweformJSON.Component jsonForm in json.Values) {

					formsDefinitions.Add(AweformFormDefinitionFromJSON(jsonForm));
				}

				return formsDefinitions;
			}

			private static AweformFormDefinition AweformFormDefinitionFromJSON(AweformJSON.Component json) {
			
				AweformFormDefinition formDefinition = new AweformFormDefinition();
				formDefinition.Id = json.GetAttributeAsInt64("id");
				formDefinition.Name = json.GetAttributeAsString("name");
				formDefinition.WorkspaceId = json.GetAttributeAsInt64("workspaceId");

				AweformJSON.Component components = json.GetAttribute("components");

				foreach (AweformJSON.Component component in components.Values) {

					AweformFormComponent formComponent = new AweformFormComponent();
					formComponent.Name = component.GetAttributeAsString("name");
					formComponent.Type = (AweformFormComponentType)Enum.Parse(typeof(AweformFormComponentType), component.GetAttributeAsString("type"));

					if (formComponent.Type == AweformFormComponentType.PictureChoice || formComponent.Type == AweformFormComponentType.Rating || formComponent.Type == AweformFormComponentType.Select || formComponent.Type == AweformFormComponentType.YesNo) {

						formComponent.Options = new List<String>();

						AweformJSON.Component options = component.GetAttribute("options");

						foreach (AweformJSON.Component option in options.Values) {

							formComponent.Options.Add(option.Value);
						}
					}

					formDefinition.Components.Add(formComponent);
				}

				return formDefinition;
			}

			private AweformJSON.Component RequestPaged(String url, Int32 from, Int32 count) {

				Int32 startPage = (from / 100);
				Int32 endPage = ((from + count - 1) / 100);

				Int32 startRequestIndex = (startPage * 100);

				Int32 page = startPage;

				AweformJSON.Component paged = null;

				while (page <= endPage) {

					AweformJSON.Component singlePage = Request(url + "?page=" + page);

					if (paged == null) {

						paged = singlePage;

					} else {

						foreach (AweformJSON.Component component in singlePage.Values) {

							paged.Values.Add(component);
						}
					}

					if (singlePage.Values.Count == 0) {

						break;
					}

					page += 1;
				}

				from -= startRequestIndex;

				if (from > 0) {

					paged.Values.RemoveRange(0, from);
				}

				if (count < paged.Values.Count) {

					paged.Values.RemoveRange(count, paged.Values.Count - count);
				}

				if (paged == null) {

					throw new AweformException("Zero pages returned");
				}

				return paged;		
			}

			protected AweformJSON.Component Request(String url) {

				WebClient wc = new WebClient();

				String response = wc.DownloadString(APIURL + url + (url.Contains("?", StringComparison.InvariantCulture) ? "&" : "?") + "apiKey=" + APIKey);
				wc.Dispose();

				AweformJSON.Component component = AweformJSON.Parse(response);

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



	//
	// AweformJSON
	// To avoid dependencies we include a simple JSON parser
	////////////////////////////////////////////////////////////////////////////////////

		public static class AweformJSON {

			public enum Token {

				EndOrUnknown,
				ObjectStart,
				ObjectEnd,
				ArrayStart,
				ArrayEnd,
				Colon,
				Comma,
				String,
				Number,
				True,
				False,
				Null
			}

			public enum ComponentType {

				String,
				Number,
				Object,
				Array,
				Boolean,
				Null
			}

			public class Component {

				public ComponentType Type { get; set; }
				public String Name { get; set; }
				public String Value { get; set; }						// String, Number, true, false, null as a String
				public List<Component> Values { get; set; }				// Attributes if "Object", Items if "Array"

				public Component(ComponentType type, String value = null) {

					Type = type;
					Values = null;
					Value = value;
				}

				public String GetAttributeAsString(String name, String defaultValue = "") {

					Component attribute = GetAttribute(name);

					if (attribute == null || attribute.Value == null) { 
				
						return defaultValue;
					}

					return attribute.Value;
				}

				public Int64 GetAttributeAsInt64(String name, Int64 defaultValue = 0) {

					Component attribute = GetAttribute(name);

					if (attribute == null || attribute.Value == null) {

						return defaultValue;
					}

					Int64 int64;

					if (Int64.TryParse(attribute.Value, out int64)) {

						return int64;

					} else {

						return defaultValue;
					}
				}

				public Component GetAttribute(String name) {

					if (Type != ComponentType.Object) {

						return null;
					}

					foreach (Component value in Values) {

						if (value.Name == name) {

							return value;
						}
					}

					return null;
				}

				public List<Component> GetAttributes() {

					if (Type != ComponentType.Object) {

						return null;
					}

					return Values;
				}
			}
		
			public static Component Parse(String json) {
						
				if (json != null) {
			
					Int32 ix = 0;
					Boolean success = true;

					Component jsonComponent = ParseValue(json.ToCharArray(), ref ix, ref success);

					if (success) {

						return jsonComponent;

					} else {

						return null;
					}
			
				} else {
			
					return null;
				}
			}

			private static Component ParseObject(Char[] chars, ref Int32 ix, ref Boolean success) {
			
				Component component = new Component(ComponentType.Object);
			
				GetNextToken(chars, ref ix); // skip {

				Boolean done = false;
			
				while (!done) {

					Token nextToken = PeekNextToken(chars, ix);

					if (nextToken == Token.EndOrUnknown) {

						success = false;
						return null;

					} else if (nextToken == Token.Comma) {

						GetNextToken(chars, ref ix); // skip ,

					} else if (nextToken == Token.ObjectEnd) {

						GetNextToken(chars, ref ix); // skip }
						return component;

					} else if (nextToken == Token.String) {

						String name = ParseString(chars, ref ix, ref success);

						if (!success) {

							return null;
						}

						nextToken = GetNextToken(chars, ref ix);

						if (nextToken != Token.Colon) {

							success = false;
							return null;
						}

						Component value = ParseValue(chars, ref ix, ref success);
					
						if (!success) {
					
							return null;
						}

						value.Name = name;

						if (component.Values == null) {

							component.Values = new List<Component>();
						}

						component.Values.Add(value);

					} else {

						success = false;
					}
				}

				return component;
			}

			private static Component ParseArray(Char[] chars, ref Int32 ix, ref Boolean success) {

				Component component = new Component(ComponentType.Array);
				component.Values = new List<Component>();

				GetNextToken(chars, ref ix); // skip [

				while (true) {

					Token token = PeekNextToken(chars, ix);

					if (token == Token.EndOrUnknown) {
				
						success = false;
						return null;

					} else if (token == Token.Comma) {

						GetNextToken(chars, ref ix); // skip ,

					} else if (token == Token.ArrayEnd) {
					
						GetNextToken(chars, ref ix); // skip ]
						break;

					} else {

						component.Values.Add(ParseValue(chars, ref ix, ref success));
				
						if (!success) {
				
							return null;
						}
					}
				}

				return component;
			}

			private static Component ParseValue(Char[] chars, ref Int32 ix, ref Boolean success) {

				Component component = null;

				Token nextToken = PeekNextToken(chars, ix);

				if (nextToken == Token.String) {

					component = new Component(ComponentType.String, ParseString(chars, ref ix, ref success));

				} else if (nextToken == Token.Number) {

					component = ParseNumber(chars, ref ix, ref success);

				} else if (nextToken == Token.ObjectStart) {

					component = ParseObject(chars, ref ix, ref success);

				} else if (nextToken == Token.ArrayStart) {

					component = ParseArray(chars, ref ix, ref success);
					
				} else if (nextToken == Token.True || nextToken == Token.False) {

					GetNextToken(chars, ref ix);

					component = new Component(ComponentType.Boolean, (nextToken == Token.True)? "true" : "false");

				} else if (nextToken == Token.Null) {

					GetNextToken(chars, ref ix);

					component = new Component(ComponentType.Null, "null");

				} else {

					success = false;
				}

				return component;
			}

			private static String ParseString(Char[] chars, ref Int32 ix, ref Boolean success) {
			
				StringBuilder sb = new StringBuilder();

				SkipWhitespace(chars, ref ix);

				ix++; // skip "

				Boolean complete = false;

				while (!complete) {

					if (ix == chars.Length) {

						break;
					}

					Char c = chars[ix++];

					if (c == '"') {

						complete = true;
						break;

					} else if (c == '\\') {

						if (ix == chars.Length) {
					
							break;
						}

						c = chars[ix++];

						if (c == '"') {

							sb.Append('"');

						} else if (c == '\\') {
						
							sb.Append('\\');

						} else if (c == '/') {
						
							sb.Append('/');

						} else if (c == 'b') {
						
							sb.Append('\b');

						} else if (c == 'f') {
						
							sb.Append('\f');

						} else if (c == 'n') {
						
							sb.Append('\n');

						} else if (c == 'r') {
						
							sb.Append('\r');

						} else if (c == 't') {
						
							sb.Append('\t');

						} else if (c == 'u') {
						
							Int32 remainingLength = chars.Length - ix;
						
							if (remainingLength >= 4) {
						
								Int32 codePoint;

								if (!Int32.TryParse(new String(chars, ix, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out codePoint)) {

									success = false;
									return "";
								}
							
								sb.Append(Char.ConvertFromUtf32((Int32)codePoint));
							
								ix += 4;

							} else {

								break;
							}
						}

					} else {

						sb.Append(c);
					}
				}

				if (!complete) {
				
					success = false;
					return null;
				}

				return sb.ToString();
			}

			private static Component ParseNumber(Char[] chars, ref Int32 ix, ref Boolean success) {

				SkipWhitespace(chars, ref ix);

				Int32 lastNumberCharacterIndex;
				String numberCharacters = "0123456789-.eE";

				for (lastNumberCharacterIndex = ix; lastNumberCharacterIndex < chars.Length; ++lastNumberCharacterIndex) {
			
					if (!numberCharacters.Contains(chars[lastNumberCharacterIndex], StringComparison.Ordinal)) {
			
						break;
					}
				}

				lastNumberCharacterIndex -= 1;

				Int32 charLength = (lastNumberCharacterIndex - ix) + 1;

				Double number;
				success = Double.TryParse(new String(chars, ix, charLength), NumberStyles.Any, CultureInfo.InvariantCulture, out number);

				ix = lastNumberCharacterIndex + 1;

				Component component = new Component(ComponentType.Number);
				component.Value = number.ToString(CultureInfo.InvariantCulture);

				return component;
			}

			private static void SkipWhitespace(Char[] chars, ref Int32 ix) {
			
				while (ix < chars.Length) {

					Char c = chars[ix];

					if (c != ' ' && c != '\t' && c != '\n' && c != '\r') {

						break;
					}

					ix++;
				}
			}

			private static Token PeekNextToken(Char[] chars, Int32 ix) {
			
				Int32 peekFromIndex = ix;
				return GetNextToken(chars, ref peekFromIndex);
			}

			private static Boolean ForwardMatch(String what, Char[] chars, ref Int32 ix) {

				Int32 remainingLength = chars.Length - ix;

				if (remainingLength >= what.Length) {

					for (Int32 i = 0; i < what.Length; ++i) {

						if (chars[ix + i] != what[i]) {

							return false;
						}
					}

					ix += what.Length;
					return true;
				}

				return false;
			}

			private static Token GetNextToken(Char[] chars, ref Int32 ix) {
			
				SkipWhitespace(chars, ref ix);

				if (ix == chars.Length) {
			
					return Token.EndOrUnknown;
				}

				Char c = chars[ix++];
			
				if (c == '{') {
					
					return Token.ObjectStart;
				
				} else if (c == '}') {

					return Token.ObjectEnd;

				} else if (c == '[') {

					return Token.ArrayStart;

				} else if (c == ']') {

					return Token.ArrayEnd;

				} else if (c == ':') {

					return Token.Colon;

				} else if (c == ',') {

					return Token.Comma;

				} else if (c == '"') {

					return Token.String;

				} else if ("-0123456789".Contains(c, StringComparison.Ordinal)) {

					return Token.Number;
				}

				ix--;

				if (ForwardMatch("false", chars, ref ix)) {

					return Token.False;

				} else if (ForwardMatch("true", chars, ref ix)) {

					return Token.True;

				} else if (ForwardMatch("null", chars, ref ix)) {

					return Token.Null;

				} else {

					return Token.EndOrUnknown;
				}
			}
		}
}