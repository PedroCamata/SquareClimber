using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public const string PLATFORM_TAG = "platform";
    public const string PLAYER_OBJ_NAME = "player";
    public const float SCREEN_WORLD_SPACE_SIZE_X = 4.5f;

    public static Transform playerTransform;

    void Start () {
        playerTransform = GameObject.Find(PLAYER_OBJ_NAME).transform;
        cameraForceAspectRatio();
    }

    private void cameraForceAspectRatio()
    {
        // set the desired aspect ratio (the values in this example are
        // hard-coded for 9:18, but you could make them into public
        // variables instead so you can set them at design time)
        float targetaspect = 0.5f; //9.0f / 18.0f;

        // determine the game window's current aspect ratio
        float windowaspect = (float)Screen.width / (float)Screen.height;

        // current viewport height should be scaled by this amount
        float scaleheight = windowaspect / targetaspect;

        // obtain camera component so we can modify its viewport
        Camera camera = GetComponent<Camera>();

        // if scaled height is less than current height, add letterbox
        if (scaleheight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            camera.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = camera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
    }
	
	void FixedUpdate() {
        cameraMovement();
	}

    private void cameraMovement()
    {
        float movementY = playerTransform.position.y - transform.position.y;

        if(transform.position.y <= 0 && movementY < 0)
        {
            return;
        }

        transform.Translate(0, movementY * Time.deltaTime, 0);

    }
}
