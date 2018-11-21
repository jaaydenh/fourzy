
QuickEvents are used just like UnityEvents (http://docs.unity3d.com/Manual/UnityEvents.html) e.g.:

	public QuickEvent OnClickEvent;
	
	OnClickEvent.AddListener(() => Debug.Log("Non persistent listener"));
	int eventCount = OnClickEvent.GetPersistentEventCount();
	OnClickEvent.Invoke();

Use the AdvancedEvent class when you need multiple arguments support in the inspector e.g.:
	
	[System.Serializable] public class AdvancedStringEvent : AdvancedEvent<string> {}
	
	public AdvancedStringEvent OnSomeEvent;
	
	// Invoke event with default argument
	OnSomeEvent.Invoke("Hello, World!");