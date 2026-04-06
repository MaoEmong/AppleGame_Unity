using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;

public class GameController : MonoBehaviour
{
	[System.Serializable]
	private class ScoreCreateRequest
	{
		public int score;
	}

	[SerializeField]
	private	GameObject		panelMainMenu;
	[SerializeField]
	private	GameObject		panelInGame;
	[SerializeField]
	private	GameObject		panelGameOver;
	[SerializeField]
	private	TextMeshProUGUI	textInGameScore;
	[SerializeField]
	private	TextMeshProUGUI	textGameOverScore;
	[SerializeField]
	private	Image			timeGauge;
	[SerializeField]
	private	float			maxTime = 120f;
	[SerializeField]
	private	string			scoreServerHost = "192.168.0.68";
	[SerializeField]
	private	int				scoreServerPort = 5050;

	private	int				currentScore = 0;
	private	AudioSource		audioSource;
	private	bool			hasSubmittedScore = false;

	public	bool			IsGameStart { private set; get; } = false;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	public void GameStart()
	{
		IsGameStart = true;

		panelMainMenu.SetActive(false);
		panelInGame.SetActive(true);
		// audioSource.Play();

		StartCoroutine(nameof(TimeCounter));
	}

	public void IncreaseScore(int addScore)
	{
		currentScore += addScore;
		textInGameScore.text = currentScore.ToString();
	}

	public void ButtonRestart()
	{
		SceneManager.LoadScene(0);
		//SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void GameOver()
	{
		if ( IsGameStart == false ) return;

		IsGameStart = false;
		textGameOverScore.text = $"SCORE\n{currentScore}";

		panelInGame.SetActive(false);
		panelGameOver.SetActive(true);
		// audioSource.Stop();

		if ( hasSubmittedScore == false )
		{
			hasSubmittedScore = true;
			StartCoroutine(nameof(SendScoreToServer));
		}
	}

	private IEnumerator TimeCounter()
	{
		float currentTime = maxTime;

		while ( currentTime > 0 )
		{
			currentTime -= Time.deltaTime;
			timeGauge.fillAmount = currentTime / maxTime;

			yield return null;
		}

		GameOver();
	}

	private IEnumerator SendScoreToServer()
	{
		string url = $"http://{scoreServerHost}:{scoreServerPort}/api/scores";
		ScoreCreateRequest requestBody = new ScoreCreateRequest { score = currentScore };
		byte[] body = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(requestBody));

		using UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
		request.uploadHandler = new UploadHandlerRaw(body);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		request.timeout = 5;

		yield return request.SendWebRequest();

		if ( request.result == UnityWebRequest.Result.Success )
		{
			Debug.Log($"GameController::SendScoreToServer() success - {request.responseCode}");
		}
		else
		{
			Debug.LogWarning($"GameController::SendScoreToServer() failed - {request.error}");
		}
	}
}

