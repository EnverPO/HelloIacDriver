namespace MyApi
{
	using System;
	using Skyline.DataMiner.ConnectorAPI.SkylineCommunications.HelloInterAppCalls.InterAppMessages.MyTable;
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;
	using Skyline.DataMiner.Core.InterAppCalls.Common.MessageExecution;
	using Skyline.DataMiner.Scripting;

	namespace MyExecutors
	{
		public class CreateRowExecutor : MessageExecutor<CreateRow>
		{
			public CreateRowExecutor(CreateRow message) : base(message)
			{
			}

			/// <summary>
			/// Creates a return Message.
			/// </summary>
			/// <returns>A message representing the response for the processed message.</returns>
			public override Message CreateReturnMessage()
			{
				return new CreateRowResponse
				{
					Success = true,
					Description = "Your credit card info isn't compromised in any way. Jolly good!",
					Guid = Message.Guid,
				};
			}

			/// <summary>
			/// Reads data from SLProtocol, Engine or other data sources.
			/// </summary>
			/// <param name="dataSource">SLProtocol, Engine, or other data sources.</param>
			public override void DataGets(object dataSource)
			{
				MyTableData cardData = Message.ExampleData;
				CreditcardsQActionRow creditCardRow = new CreditcardsQActionRow
				{
					Cardnumber_1001 = cardData.CreditCardNumber,
					Cardholder_1002 = string.Format("{0} {1}", cardData.FirstName, cardData.LastName),
					Cardexpirationdate_1003 = string.Format("{0}/{1}", cardData.ExpirationMonth, cardData.ExpirationYear),
					Mothermaidenname_1004 = cardData.MotherMaidenName,
					Messagetimesent_1005 = cardData.RequestSendTime.ToOADate(),
					Messagetimereceived_1006 = DateTime.Now.ToOADate(),
				};

				SLProtocol protocol = dataSource as SLProtocol; // Can be replaced if other dataSource type is used.
				if (protocol != null)
				{
					if (!protocol.Exists(Parameter.Creditcards.tablePid, cardData.CreditCardNumber))
					{
						protocol.AddRow(Parameter.Creditcards.tablePid, creditCardRow.ToObjectArray());
					}
				}
			}

			/// <summary>
			/// Writes data to SLProtocol, Engine, or another data destination.
			/// </summary>
			/// <param name="dataDestination">SLProtocol, Engine, or another data destination.</param>
			public override void DataSets(object dataDestination)
			{
				// Do nothing
			}

			/// <summary>
			/// Modifies retrieved data and Message data into a correct format for setting.
			/// </summary>
			public override void Modify()
			{
				// Do nothing
			}

			/// <summary>
			/// Parses the data retrieved from a data source in DataGets.
			/// </summary>
			public override void Parse()
			{
				// Do nothing
			}

			/// <summary>
			/// Validates received data for validity.
			/// </summary>
			/// <returns>A boolean indicating if the received data is valid.</returns>
			public override bool Validate()
			{
				return true;
			}
		}
	}
}