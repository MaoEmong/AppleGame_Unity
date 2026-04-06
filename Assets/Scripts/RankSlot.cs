using TMPro;
using UnityEngine;

public class RankSlot : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI textRank;

	public void Init(int rank, string ip, int score)
	{
		string displayIp = string.IsNullOrWhiteSpace(ip) ? "-" : ip;
		textRank.text = $"[{rank}등]  IP: {displayIp} | 점수: {score}";
	}

	public void SetMessage(string message)
	{
		textRank.text = message;
	}
}
