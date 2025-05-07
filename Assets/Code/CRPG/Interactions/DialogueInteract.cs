

namespace CRPG.Interactions
{
    class DialogueInteract : Interact
    {
		DialogueActor _dialogueActor;
		GameManager _gameManager;

		public DialogueInteract(DialogueActor dialogueActor, GameManager gameManager)
		{
			_dialogueActor = dialogueActor;
			_gameManager = gameManager;
		}

		public override void Execute(PersonageController executor)
		{
			_gameManager.StartDialogue(_dialogueActor);
		}
	}
}