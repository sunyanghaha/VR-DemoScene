using UnityEngine;
using System.Collections;


public class cameraManipulate : MonoBehaviour {

    // How fast to move object
    public float m_moveSpeed = 10.0f;
    // Stick's move particle
    public ParticleSystem m_moveParticle = null;
    // Stick's rotate particle
    public ParticleSystem m_rotateParticle = null;

    private GameObject m_selectObj = null;
    private float m_objDistance = 0.0f;
    private GameObject m_rotateArrow = null;
    private GameObject m_player = null;
    private Vector3 m_playerPos = Vector3.zero;
    private shakingObject m_stick = null;

    private bool m_fire1Pressed = false;
    private bool m_fire2Pressed = false;

	// Use this for initialization
	void Start () {
        m_player = GameObject.Find("First Person Controller");
        if (m_player == null)
            Debug.LogError("No player controller in scene.");
        m_rotateArrow = GameObject.Find("RotateArrow");
        GameObject stick = GameObject.Find("Stick");
        if (stick != null && stick.GetComponent<shakingObject>() != null)
            m_stick = stick.GetComponent<shakingObject>();

	}
	
	// Update is called once per frame
	void Update () {
        
        // Make sure two mays of manipulate not interact with each other
        if (!m_fire1Pressed)
            JudgeFire2();
        if (!m_fire2Pressed)
            JudgeFire1();

	}

    // Judge if player want to move object
    private void JudgeFire1()
    {
        // Start to move
        if (Input.GetAxis("Fire1") > 0 && !m_fire1Pressed)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                if (hit.transform.gameObject.GetComponent<draggableObject>() != null)
                {
                    if (m_selectObj != null)
                    {
                        m_selectObj.GetComponent<draggableObject>().DeselectObject();
                    }
                    if (m_stick != null)
                    {
                        m_stick.SetShaking(false, Color.green);
                    }
                    m_selectObj = hit.transform.gameObject;
                    m_selectObj.GetComponent<draggableObject>().SelectObject();
                    m_moveParticle.gameObject.SetActive(true);
                    m_objDistance = Vector3.Distance(transform.position, m_selectObj.transform.position);
                    m_fire1Pressed = true;
                }
            }
        }

        // Moving
        if (Input.GetAxis("Fire1") > 0 && m_fire1Pressed && m_selectObj != null)
        {
            float newDistance = m_objDistance;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, m_objDistance))
            {
                newDistance = hit.distance - 0.5f;
            }
            m_selectObj.rigidbody.velocity = m_moveSpeed * (newDistance / m_objDistance) *
                (transform.position + newDistance * transform.forward - m_selectObj.transform.position);
        }

        // End of move
        if (Input.GetAxis("Fire1") < 1 && m_fire1Pressed && m_selectObj != null)
        {
            m_selectObj.rigidbody.velocity = m_selectObj.rigidbody.velocity / 5;
            m_selectObj.GetComponent<draggableObject>().DeselectObject();
            m_selectObj = null;
            m_moveParticle.gameObject.SetActive(false);
            m_fire1Pressed = false;
            if (m_stick != null)
            {
                m_stick.SetShaking(true, Color.cyan);
            }
        }
    }

    // Judge if player want to rotate object
    private void JudgeFire2()
    {
        // Start of rotating
        if (Input.GetAxis("Fire2") > 0 && !m_fire2Pressed)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                if (hit.transform.gameObject.GetComponent<draggableObject>() != null)
                {
                    if (m_selectObj != null)
                    {
                        m_selectObj.GetComponent<draggableObject>().DeselectObject();
                    }
                    m_selectObj = hit.transform.gameObject;
                    m_selectObj.GetComponent<draggableObject>().StartRotate();
                    m_rotateParticle.gameObject.SetActive(true);
                    m_playerPos = m_player.transform.position;
                    m_rotateArrow.transform.position = m_selectObj.transform.position;
                    m_fire2Pressed = true;
                    if (m_stick != null)
                    {
                        m_stick.SetShaking(false, Color.yellow);
                    }
                }
            }
        }

        // Rotating
        if (Input.GetAxis("Fire2") > 0 && m_fire2Pressed && m_selectObj != null)
        {

            m_player.transform.position = m_playerPos;
            Vector3 v1 = m_selectObj.transform.position - transform.position;
            Vector3 v2 = transform.forward;
            float dis = Vector3.Dot(v1, v2) / Vector3.Dot(v2, v2);
            Vector3 newright = v2 * dis - v1;
            float zAngle = Vector3.Angle(transform.right, newright);
            if ((v2 * dis - v1).y < 0)
                zAngle = 360 - zAngle;
            Vector3 eular = transform.eulerAngles;
            eular.x = 0;
            eular.z = zAngle;
            m_rotateArrow.transform.rotation = Quaternion.Euler(eular);
            if (newright.magnitude > 3)
                m_rotateArrow.transform.localScale = Vector3.one;
            else
                m_rotateArrow.transform.localScale = Vector3.one * newright.magnitude / 3;
        }

        // End of rotate, really rotate objects
        if (Input.GetAxis("Fire2") < 1 && m_fire2Pressed && m_selectObj != null)
        {
            m_selectObj.GetComponent<draggableObject>().RotateObject(m_rotateArrow.transform.eulerAngles.z);
            m_selectObj.GetComponent<draggableObject>().DeselectObject();
            m_selectObj = null;
            m_rotateParticle.gameObject.SetActive(false);
            m_rotateArrow.transform.localScale = Vector3.zero;
            m_fire2Pressed = false;
            if (m_stick != null)
            {
                m_stick.SetShaking(true, Color.cyan);
            }
        }
    }
}
