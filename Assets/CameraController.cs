using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public float x = 0.0f, y = 1.0f, z = 0.0f;
	public float speed = 2.0f;
	private Vector3 target;

	// Use this for initialization
	void Start ()
	{
		Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
			var dependencyStatus = task.Result;
			if (dependencyStatus == Firebase.DependencyStatus.Available)
			{
				// Create and hold a reference to your FirebaseApp, i.e.
				//   app = Firebase.FirebaseApp.DefaultInstance;
				// where app is a Firebase.FirebaseApp property of your application class.

				// Set a flag here indicating that Firebase is ready to use by your
				// application.
			}
			else
			{
				UnityEngine.Debug.LogError(System.String.Format(
					"Could not resolve all Firebase dependencies: {0}", dependencyStatus));
				// Firebase Unity SDK is not safe to use here.
			}
		});

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
					Debug.Log("Setup: " + snapshot.Value);
					// target = new Vector3(snapshot.x, 0, snapshot.y);
				}
			});
		
		target = new Vector3(x, y, z);
	}

	// Update is called once per frame
	void Update ()
	{
		_HandleSimulatorLifecycle();
		FirebaseDatabase.DefaultInstance
			.GetReference("locationPosition")
			.ValueChanged += _HandleValueChanged;

		// transform.position = target;
		transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
	}

	

	private void _HandleValueChanged(object sender, ValueChangedEventArgs args)
	{
		double x = 0.0, y = 0.0;
		Double xx = 0, yy = 0;
		if (args.DatabaseError != null)
		{
			return;
		}

		foreach (KeyValuePair<string, object> pair in (IEnumerable<KeyValuePair<string, object>>) args.Snapshot.Value)
		{
			if (pair.Key == "x")
			{
				// x = (double) pair.Value;
				xx = (Double) pair.Value;
				Debug.Log(pair.Value.GetType());
			} else if (pair.Key == "y")
			{
				yy = (Double) pair.Value;
				// y = (Double) pair.Value;
			}
		}

		target = new Vector3((float)xx , 1, (float)yy);
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
