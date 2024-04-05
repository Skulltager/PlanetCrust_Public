namespace SheetCodes
{
	//Generated code, do not edit!

	public enum PopupMessageIdentifier
	{
		[Identifier("None")] None = 0,
		[Identifier("Kicked From Lobby")] KickedFromLobby = 1,
		[Identifier("Incorrect Password")] IncorrectPassword = 2,
		[Identifier("Disconnected From  Server")] DisconnectedFromServer = 3,
		[Identifier("Host Closed Lobby")] HostClosedLobby = 4,
		[Identifier("Failed To Connect To Master")] FailedToConnectToMaster = 5,
		[Identifier("Server Full")] ServerFull = 6,
	}

	public static class PopupMessageIdentifierExtension
	{
		public static PopupMessageRecord GetRecord(this PopupMessageIdentifier identifier, bool editableRecord = false)
		{
			return ModelManager.PopupMessageModel.GetRecord(identifier, editableRecord);
		}
	}
}
