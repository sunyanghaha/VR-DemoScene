using UnityEngine;
using System.Collections;

public class draggableObject : MonoBehaviour {

    // Respawn deadline
    public int m_respawnBelow = -10;

    // Rotate Speed Ratio
    public float m_rotateSpeed = 200.0f;

    private Material m_selectedMaterial = null;
    private Shader m_oriShader = null;
    private Vector3 m_oriPos = Vector3.zero;
    private GameObject m_trailParticle = null;

    private Vector3 m_rotateAxis = Vector3.zero;
    private float m_angleLeft = 0.0f;

	// Use this for initialization
	void Start () {
        m_selectedMaterial = (Material)Resources.Load("HighLight");
        if(m_selectedMaterial == null)
        {
            Debug.LogError("missing highlight material");
        }
        m_oriShader = gameObject.renderer.material.shader;
        m_oriPos = transform.position;
        m_trailParticle = GameObject.Find("ObjectTrail");
	}
	
	// Update is called once per frame
	void Update () {

        // Respawn if object fly out of playable area
	    if(transform.position.y < m_respawnBelow)
        {
            transform.position = m_oriPos;
            rigidbody.velocity = Vector3.zero;
        }

        // If rotating, rotate gradually
        if(m_angleLeft > 0)
        {
            float angle = m_rotateSpeed * Time.deltaTime;
            if (m_angleLeft < angle)
            {
                angle = m_angleLeft;
                m_angleLeft = 0;
            }
            else
            {
                m_angleLeft -= angle;
            }
            transform.Rotate(m_rotateAxis, angle);
        }
	}

    // Called before Movement, setup correct outline shader
    public void SelectObject()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        gameObject.renderer.material.shader = m_selectedMaterial.shader;
        gameObject.renderer.material.SetColor("_OutlineColor", Color.green);
        m_trailParticle.SetActive(true);
        m_trailParticle.transform.position = transform.position;
        m_trailParticle.transform.parent = transform;
    }

    // Called after finish manipulate. Reset shader
    public void DeselectObject()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
        gameObject.renderer.material.shader = m_oriShader;
        m_trailParticle.SetActive(false);
    }

    // Called before Rotation, setup correct outline shader
    public void StartRotate()
    {
        
        gameObject.renderer.material.shader = m_selectedMaterial.shader;
        gameObject.renderer.material.SetColor("_OutlineColor", Color.yellow);
    }

    // Rotate object according right vector angle
    // Maybe not so appropriate, but it can be polished if more time are given
    public void RotateObject(float angle)
    {
        if(angle < 45 || angle >= 315)
        {
            m_rotateAxis = -Vector3.up;
        }
        else if(angle >= 45 && angle < 135)
        {
            m_rotateAxis = Vector3.right;
        }
        else if(angle >= 135 && angle < 225)
        {
            m_rotateAxis = Vector3.up;
        }
        else
        {
            m_rotateAxis = -Vector3.right;
        }
        m_angleLeft = 90;

    }
}
