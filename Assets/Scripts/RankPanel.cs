using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class RankPanel : MonoBehaviour
{
	[System.Serializable]
	private class RankingEntry
	{
		public int rank;
		public string ip;
		public int score;
	}

	[System.Serializable]
	private class RankingResponse
	{
		public RankingEntry[] rankings;
	}

	[SerializeField]
	private RankSlot rankSlotPrefab;
	[SerializeField]
	private Transform rankSlotParent;
	[SerializeField]
	private string scoreServerHost = "192.168.0.68";
	[SerializeField]
	private int scoreServerPort = 5050;
	[SerializeField]
	private int rankingLimit = 10;

	private Coroutine refreshCoroutine;

	private void OnEnable()
	{
		refreshCoroutine = StartCoroutine(nameof(RefreshRankings));
	}

	private void OnDisable()
	{
		if ( refreshCoroutine != null )
		{
			StopCoroutine(refreshCoroutine);
			refreshCoroutine = null;
		}
	}

	private IEnumerator RefreshRankings()
	{
		ClearRankSlots();

		string url = $"http://{scoreServerHost}:{scoreServerPort}/api/scores/rankings?limit={rankingLimit}";
		using UnityWebRequest request = UnityWebRequest.Get(url);
		request.timeout = 5;

		yield return request.SendWebRequest();

		refreshCoroutine = null;

		if ( request.result != UnityWebRequest.Result.Success )
		{
			Debug.LogWarning($"RankPanel::RefreshRankings() failed - {request.error}");
			CreateMessageSlot("랭킹을 불러오지 못했습니다.");
			yield break;
		}

		RankingResponse response = JsonUtility.FromJson<RankingResponse>(request.downloadHandler.text);
		if ( response == null || response.rankings == null || response.rankings.Length == 0 )
		{
			CreateMessageSlot("등록된 랭킹이 없습니다.");
			yield break;
		}

		for ( int i = 0; i < response.rankings.Length; ++i )
		{
			RankingEntry entry = response.rankings[i];
			RankSlot slot = Instantiate(rankSlotPrefab, rankSlotParent);
			int rank = entry.rank > 0 ? entry.rank : i + 1;
			slot.Init(rank, entry.ip, entry.score);
		}
	}

	private void ClearRankSlots()
	{
		for ( int i = rankSlotParent.childCount - 1; i >= 0; --i )
		{
			Destroy(rankSlotParent.GetChild(i).gameObject);
		}
	}

	private void CreateMessageSlot(string message)
	{
		RankSlot slot = Instantiate(rankSlotPrefab, rankSlotParent);
		slot.SetMessage(message);
	}
}
