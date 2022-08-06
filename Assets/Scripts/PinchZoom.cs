using UnityEngine;

public class PinchZoom : MonoBehaviour
{
	public delegate void CameraSizeChangeHandler();
	public static event CameraSizeChangeHandler CameraSizeChanged;

    public float orthoZoomSpeed;        // The rate of change of the orthographic size in orthographic mode.

	public SpriteRenderer bgSpriteRenderer;

	private Camera mainC;

    void Start()
    {
		mainC = ValueStore.Instance.mainCamera;
    }

    void Update()
    {
        Touch[] toches = Input.touches;

        if(Input.touchCount == 1)
        {
            if (toches[0].phase == TouchPhase.Moved)
            {
                Vector3 bgsize = bgSpriteRenderer.bounds.size;
				float height = 2f * mainC.orthographicSize;
				float width = height * mainC.aspect;
                Vector2 delta = toches[0].deltaPosition;
				float posX = -delta.x * mainC.orthographicSize/1.7f * Time.deltaTime;            
				float posY = -delta.y * mainC.orthographicSize/1.7f * Time.deltaTime;

				Vector3 xExtent = new Vector3(bgSpriteRenderer.bounds.extents.x, 0, 0);
				Vector3 yExtent = new Vector3(0, bgSpriteRenderer.bounds.extents.y, 0);
				Vector3 bgPos = bgSpriteRenderer.transform.position;

				transform.position = new Vector3(Mathf.Clamp(transform.position.x + posX, (bgPos - xExtent).x + width / 2, (bgPos + xExtent).x - width / 2)
					, Mathf.Clamp(transform.position.y + posY, (bgPos - yExtent).y + height / 2, (bgPos + yExtent).y - height / 2), transform.position.z);
            }
        }else if (Input.touchCount == 2)
        {
			Vector3 bgsize = bgSpriteRenderer.bounds.size;

            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame. 
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

			//mainC.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;
			SetCameraSize(mainC, Mathf.Clamp(mainC.orthographicSize + deltaMagnitudeDiff * orthoZoomSpeed, 20f, (bgSpriteRenderer.bounds.size.y / 2))); 

			Vector3 xExtent = new Vector3(bgSpriteRenderer.bounds.extents.x, 0, 0);
			Vector3 yExtent = new Vector3(0, bgSpriteRenderer.bounds.extents.y, 0);
			Vector3 bgPos = bgSpriteRenderer.transform.position;

			float height = 2f * mainC.orthographicSize;
			float width = height * mainC.aspect;

			transform.position = new Vector3(Mathf.Clamp(transform.position.x, (bgPos - xExtent).x + width / 2, (bgPos + xExtent).x - width / 2)
				, Mathf.Clamp(transform.position.y, (bgPos - yExtent).y + height / 2, (bgPos + yExtent).y - height / 2), transform.position.z);
        }
    }

	public static void SetCameraSize(Camera cam, float size){
		cam.orthographicSize = size;
		if (CameraSizeChanged != null) {
			CameraSizeChanged ();
		}
	}
}