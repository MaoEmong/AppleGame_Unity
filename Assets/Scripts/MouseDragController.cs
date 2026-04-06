using System.Collections.Generic;
using UnityEngine;

public class MouseDragController : MonoBehaviour
{
	[SerializeField]
	private	GameController		gameController;
	[SerializeField]
	private	AppleSpawner		appleSpawner;
	[SerializeField]
	private	RectTransform		dragRectangle;

	private	int					sum = 0;
	private	Rect				dragRect;
	private	Vector2				start = Vector2.zero;
	private	Vector2				end = Vector2.zero;
	private	AudioSource			audioSource;
	private	List<Apple>			selectedAppleList = new List<Apple>();

	private void Awake()
	{
		dragRect	= new Rect();
		audioSource	= GetComponent<AudioSource>();

		// start, end가 (0, 0)인 상태로 이미지의 크기를 (0, 0)으로 설정해 화면에 보이지 않도록 함
		DrawDragRectangle();
	}

	private void Update()
	{
		if ( gameController.IsGameStart == false ) return;

		if ( Input.GetMouseButtonDown(0) )
		{
			start = Input.mousePosition;

			dragRect.Set(0, 0, 0, 0);
		}

		if ( Input.GetMouseButton(0) )
		{
			end = Input.mousePosition;

			// 마우스를 클릭한 상태로 드래그하는 동안 드래그 범위를 이미지로 표현
			DrawDragRectangle();
			CalculateDragRect();
			SelectApples();
		}

		if ( Input.GetMouseButtonUp(0) )
		{
			if ( sum == 10 )
			{
				int score = 0;
				foreach ( Apple apple in selectedAppleList )
				{
					score ++;
					appleSpawner.DestroyApple(apple);
				}

				// audioSource.Play();
				gameController.IncreaseScore(score);
			}
			else
			{
				// 선택된 사과들을 선택 해제
				foreach ( Apple apple in selectedAppleList )
				{
					apple.OnDeselected();
				}
			}

			// 마우스 클릭을 종료할 때 드래그 범위가 보이지 않도록
			// start, end 위치를 (0, 0)으로 설정하고 드래그 범위를 그린다
			start = end = Vector2.zero;
			DrawDragRectangle();
		}
	}

	private void DrawDragRectangle()
	{
		// 드래그 범위를 나타내는 Image UI의 위치
		dragRectangle.position = (start + end) * 0.5f;
		// 드래그 범위를 나타내는 Image UI의 크기
		dragRectangle.sizeDelta = new Vector2(Mathf.Abs(start.x - end.x), Mathf.Abs(start.y - end.y));
	}

	private void CalculateDragRect()
	{
		if ( Input.mousePosition.x < start.x )
		{
			dragRect.xMin = Input.mousePosition.x;
			dragRect.xMax = start.x;
		}
		else
		{
			dragRect.xMin = start.x;
			dragRect.xMax = Input.mousePosition.x;
		}

		if ( Input.mousePosition.y < start.y )
		{
			dragRect.yMin = Input.mousePosition.y;
			dragRect.yMax = start.y;
		}
		else
		{
			dragRect.yMin = start.y;
			dragRect.yMax = Input.mousePosition.y;
		}
	}

	private void SelectApples()
	{
		sum = 0;
		selectedAppleList.Clear();
		foreach ( Apple apple in appleSpawner.AppleList )
		{
			// 사과의 중심을 드래그 영역에 포함해야 함
			if ( dragRect.Contains(apple.Position) )
			{
				apple.OnSelected();
				selectedAppleList.Add(apple);
				sum += apple.Number;
			}
			else
			{
				apple.OnDeselected();
			}
		}
	}
}

