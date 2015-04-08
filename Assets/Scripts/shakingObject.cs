using UnityEngine;
using System.Collections;

public class shakingObject : MonoBehaviour
{
    // How fast the object shakes
    public float m_rate = 0.1f;

    private bool m_shaking = true;
    private Vector3 m_oriPos = Vector3.zero;
    private float m_angle = 0.0f;
	// Use this for initialization
	void Start () {
        m_oriPos = transform.localPosition;
        m_angle = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
	    if(m_shaking == true)
        {
            transform.localPosition = m_oriPos + new Vector3(0, Mathf.Sin(m_angle) * m_rate, 0);
            m_angle += Time.deltaTime * 3;
            if (m_angle > 360)
                m_angle -= 360;
        }
	}

    // Set the stick shake status and color
    public void SetShaking(bool shake, Color objColor)
    {
        if(shake == false)
        {
            transform.localPosition = m_oriPos;
            m_angle = 0;
        }
        gameObject.renderer.material.color = objColor;
        m_shaking = shake;
    }
}
