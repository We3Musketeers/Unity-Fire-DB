using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

	private CharacterController cc;
	public float x = 0.0f;
	public float y = 0.0f;
	public float z = 0.0f;
	public float speed = 10.0f;
	private Vector3 target;

	// Use this for initialization
	void Start ()
	{
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://estimoteindoor.firebaseio.com/");
		DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

		FirebaseDatabase.DefaultInstance
			.GetReference("locationPosition")
			.GetValueAsync().ContinueWith(task => {
				if (task.IsFaulted)
				{
				   Debug.Log("Error..");
				}
				else if (task.IsCompleted)
				{
					DataSnapshot snapshot = task.Result;
					Debug.Log(snapshot);
				    target = new Vector3(snapshot.x, 0, snapshot.y);
				}
			});

        cc = GetComponent<CharacterController>();
		target = new Vector3(x, y, z);
	}

    // Update is called once per frame
    void Update ()
    {
        _HandleSimulatorLifecycle();

		target = new Vector3(x, y, z);
        FirebaseDatabase.DefaultInstance
            .GetReference("locationPosition")
            .ValueChanged += _HandleValueChanged;

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
	}

    

    private void _HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            return;
        }

        // Do something with the data in args.Snapshot
        target = new Vector3(args.Snapshot.x, 0, args.Snapshot.y);
    }

    private void _HandleSimulatorLifecycle()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            FirebaseDatabase.DefaultInstance
                .GetReference("locationPosition")
                .ValueChanged -= _HandleValueChanged; // unsubscribe from ValueChanged.
            Application.Quit();
        }
    }
}
