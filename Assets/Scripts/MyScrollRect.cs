using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.IO;

public class MyScrollRect : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public GameObject content;

	public bool vertical, horizontal;
	private bool dragging, damp;

	private RectTransform rt;
	private RectTransform ct;

	private Camera mainCamera;

	private float timeLastDrag;

	private Vector2 lastDragDistance;
	private Vector2 lastContentPosition;
	private float lastTouchDeltaPosY = 0;
	private Vector2 lastTouchPos = new Vector2 (0, 0);

	private float yVelocity = 0f;

	private Vector2 currentVelocity;

	private Vector3[] panelCorners = new Vector3[4];
	private Vector3[] contentCorners = new Vector3[4];

	private Vector2 lastMousePos = new Vector2(0, 0);

	private Vector2 contentStartPos;
	private Vector2 dragStartPos;

	void Start(){
		rt = GetComponent<RectTransform> ();
		ct = content.GetComponent<RectTransform> ();

		rt.GetWorldCorners (panelCorners);

		mainCamera = Camera.main;
		QualitySettings.vSyncCount = 0;

	}
				
	public void OnBeginDrag (PointerEventData eventData)
	{
		dragStartPos = eventData.position;
		contentStartPos = content.transform.position;
		dragging = true;
	}
		
	public void OnDrag (PointerEventData eventData)
	{
		Vector2 startDelta = eventData.position - dragStartPos;

		Vector2 contentNewPos = content.transform.position;

		ct.GetWorldCorners (contentCorners);

		Vector2 offset = (contentStartPos + startDelta) - (Vector2)content.transform.position;

		Vector2 newbottomLeftCorner = (Vector2)contentCorners [0] + offset;
		Vector2 newbottomRightCorner = (Vector2)contentCorners [3] + offset;
		Vector2 newtopLeftCorner = (Vector2)contentCorners [1] + offset;
		Vector2 newtopRightCorner = (Vector2)contentCorners [2] + offset;

		if (vertical) {
			if (WithinBounds(Array.ConvertAll(panelCorners, item => (Vector2)item), 
				new Vector2[]{newbottomLeftCorner, newtopLeftCorner, newtopRightCorner, newbottomRightCorner})) {
				contentNewPos.y = contentStartPos.y + startDelta.y;
				currentVelocity.y = offset.y;
			}
		}

		if ((Vector2)content.transform.position == contentNewPos) {
			currentVelocity = Vector2.zero;
			return;
		}

		content.transform.position = contentNewPos;
	}

	public void OnEndDrag(PointerEventData eventData){
		dragging = false;
	}


	void FixedUpdate () {
		if (!dragging) {
			ct.GetWorldCorners (contentCorners);
			  
			//float velocity = lastDragDistance.y / timeLastDrag;
			currentVelocity.Scale(new Vector2(0.865f, 0.865f));
			float newY = content.transform.position.y + currentVelocity.y;
			// velocity = -decceleration * time
			Vector2 offset = new Vector2 (0, newY - content.transform.position.y);

			Vector2 newbottomLeftCorner = (Vector2)contentCorners [0] + offset;
			Vector2 newbottomRightCorner = (Vector2)contentCorners [3] + offset;
			Vector2 newtopLeftCorner = (Vector2)contentCorners [1] + offset;
			Vector2 newtopRightCorner = (Vector2)contentCorners [2] + offset;

			if (WithinBounds (Array.ConvertAll (panelCorners, item => (Vector2)item),
				   new Vector2[]{ newbottomLeftCorner, newtopLeftCorner, newtopRightCorner, newbottomRightCorner })) {
				content.transform.position = new Vector3 (content.transform.position.x, newY, content.transform.position.z);
			}
			
		} 
	}

	public bool WithinBounds(Vector2[] panelCorners, Vector2[] contentCorners){
		if (contentCorners[0].y <= panelCorners [0].y && contentCorners[3].y <= panelCorners [3].y
			&& contentCorners[1].y >= panelCorners [1].y && contentCorners[2].y >= panelCorners [2].y) {
			return true;
		}
		return false;
	}

	public void OnMouseClick(){
		Vector3[] contentCorners = new Vector3[4];
		ct.GetWorldCorners (contentCorners);

		Vector3[] panelCorners = new Vector3[4];
		rt.GetWorldCorners (panelCorners);

		Vector2 yOffset = new Vector2 (0, 0);

		Vector2 newbottomLeftCorner = (Vector2)contentCorners [0] + yOffset;//rt.position + rt.sizeDelta.x + yVector;
		Vector2 newbottomRightCorner = (Vector2)contentCorners[3] + yOffset;//rt.rect.min + new Vector2 (rt.rect.width, 0) + yVector;
		Vector2 newtopLeftCorner = (Vector2)contentCorners[1] + yOffset;//rt.rect.max - new Vector2 (rt.rect.width, 0) + yVector;
		Vector2 newtopRightCorner = (Vector2)contentCorners[2] + yOffset;

		print ("newTopRight: " + newtopRightCorner + "...PanelTopRight: " + panelCorners[2]);

		print ("TopLeft Content Corner: " + newtopLeftCorner + "   TopLeft Panel Corner: " + panelCorners [1]);

		print ("Mouse Pos: " + Input.mousePosition);

		if (RectTransformUtility.RectangleContainsScreenPoint(rt, Input.mousePosition)) {
			print ("MinX: " + ct.rect.xMin + "\nMaxX: " + ct.rect.xMax + "\nMinY: " + ct.rect.yMin + "\nMaxY: " + ct.rect.yMax + "\nCenter: " + ct.rect.center);
			print ("MinX: " + rt.rect.xMin + "\nMaxX: " + rt.rect.xMax + "\nMinY: " + rt.rect.yMin + "\nMaxY: " + rt.rect.yMax + "\nCenter: " + rt.rect.center);
			print (rt.rect.min);
		}
	}
}
