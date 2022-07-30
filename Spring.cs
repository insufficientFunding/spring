using UnityEngine;

namespace Malware.Springs
{
    public class FloatSpring
    {
        public FloatSpring (float initial, float s, float d)
        {
            this.time = Time.time;
            this.position = initial;
            this.velocity = 0 * initial;
            this.target = initial;

            this.Speed = s;
            this.Damper = d;
        }

        float time;
        float position;
        float velocity;
        float target;

        float damper = 1;
        float speed = 1;

        /// <summary>
        /// The value of the spring, at the time of the request.
        /// </summary>
        public float Value
        {
            get
            {
                PositionAndVelocity pv = PositionVelocity (Time.time);

                return pv.Position;
            }
            set
            {
                PositionAndVelocity pv = PositionVelocity (Time.time);

                position = value;
                velocity = pv.Velocity;
                time = Time.time;
            }
        }

        /// <summary>
        /// The velocity of the spring, at the time of the request.
        /// </summary>
        public float Velocity
        {
            get
            {
                PositionAndVelocity pv = PositionVelocity (Time.time);

                return pv.Velocity;
            }
            set
            {
                PositionAndVelocity pv = PositionVelocity (Time.time);

                position = pv.Position;
                velocity = value;
                time = Time.time;
            }
        }

        /// <summary>
        /// The acceleration of the spring, at the time of the request. Settings the acceleration will act as a push for the spring.
        /// </summary>
        public float Acceleration
        {
            get
            {
                float x = Time.time - time;
                float c0 = position = target;
                if (speed == 0)
                    return 0f;
                else if (damper < 1)
                {
                    float c = Mathf.Sqrt (1 - Mathf.Pow (Damper, 2f));
                    float c1 = (velocity / speed + damper * c0) / c;

                    return Mathf.Pow (speed, 2f) * (c0 - 2 * c1 + c1 * speed * x) / Mathf.Exp (speed * x);
                }
                else
                {
                    float c1 = velocity / speed + c0;
                    return speed * speed * (c0 - 2 * c1 + c1 * speed * x) / Mathf.Pow (2.718281828459045f, speed * x);
                }
            }
            set
            {
                PositionAndVelocity pv = PositionVelocity (Time.time);

                position = pv.Position;
                velocity = pv.Velocity + value;
                time = Time.time;
            }
        }

        /// <summary>
        /// The target value of the spring.
        /// </summary>
        public float Target
        {
            get
            {
                return target;
            }
            set
            {
                PositionAndVelocity pv = PositionVelocity (Time.time);

                position = pv.Position;
                target = value;
                time = Time.time;
            }
        }


        /// <summary>
        /// The damper of the spring, a range from 0 to 1, with 0 having no damping (spring never stops overshooting), to 1 having max damping (no overshooting).
        /// </summary>
        public float Damper
        {
            get
            {
                return damper;
            }
            set
            {
                PositionAndVelocity pv = PositionVelocity (Time.time);

                damper = Mathf.Clamp01 (value);
                time = Time.time;

                position = pv.Position;
                velocity = pv.Velocity;
            }
        }

        /// <summary>
        /// The speed of the spring.
        /// </summary>
        public float Speed
        {
            get
            {
                return speed;
            }
            set
            {
                PositionAndVelocity pv = PositionVelocity (Time.time);

                speed = Mathf.Max (value, 0);
                time = Time.time;

                position = pv.Position;
                velocity = pv.Velocity;
            }
        }

        PositionAndVelocity PositionVelocity (float tick)
        {
            PositionAndVelocity returned = new PositionAndVelocity ();

            float x = tick - time;
            float c0 = position - target;

            if (speed == 0)
            {
                returned.Position = position;
                returned.Velocity = 0f;

                return returned;
            }
            else if (damper < 1)
            {
                float c = Mathf.Sqrt (1 - Mathf.Pow (damper, 2f));
                float c1 = (velocity / speed + damper * c0) / c;
                float co = Mathf.Cos (c * speed * x);
                float si = Mathf.Sin (c * speed * x);
                float e = Mathf.Exp (damper * speed * x);

                returned.Position = target + (c0 * co + c1 * si) / e;
                returned.Velocity = speed * ((c * c1 - damper * c0) * co - (c * c0 + damper * c1) * si) / e;

                return returned;
            }
            else
            {
                float c1 = velocity / speed + c0;
                float e = Mathf.Exp (speed * x);

                returned.Position = target + (c0 + c1 * speed * x) / e;
                returned.Velocity = speed * (c1 - c0 - c1 * speed * x) / e;

                return returned;
            }
        }

        /// <summary>
        /// Pushes the spring with the input acceleration.
        /// </summary>
        public void Accelerate (float acceleration)
        {
            float newTick = Time.time;
            PositionAndVelocity pv = PositionVelocity (newTick);
            position = pv.Position;
            velocity = pv.Velocity + acceleration;
            time = newTick;
        }

        private class PositionAndVelocity
        {
            public float Position;
            public float Velocity;
        }
    }

    public class Vector3Spring
    {

        public Vector3Spring (Vector3 initial, float springSpeed, float springDampening)
        {
            xSpring = new FloatSpring (initial.x, springSpeed, springDampening);
            ySpring = new FloatSpring (initial.y, springSpeed, springDampening);
            zSpring = new FloatSpring (initial.z, springSpeed, springDampening);
        }

        private FloatSpring xSpring;
        private FloatSpring ySpring;
        private FloatSpring zSpring;

        /// <summary>
        /// The position of the spring, at the time of the request.
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return new Vector3 (xSpring.Value, ySpring.Value, zSpring.Value);
            }
            set
            {
                xSpring.Value = value.x;
                ySpring.Value = value.y;
                zSpring.Value = value.z;
            }
        }
        /// <summary>
        /// The velocity of the spring, at the time of the request.
        /// </summary>
        public Vector3 Velocity
        {
            get
            {
                return new Vector3 (xSpring.Velocity, ySpring.Velocity, zSpring.Velocity);
            }
            set
            {
                xSpring.Velocity = value.x;
                ySpring.Velocity = value.y;
                zSpring.Velocity = value.z;
            }
        }

        /// <summary>
        /// The acceleration of the spring, at the time of the request. Settings the acceleration will act as a push for the spring.
        /// </summary>
        public Vector3 Acceleration
        {
            get
            {
                return new Vector3 (xSpring.Acceleration, ySpring.Acceleration, zSpring.Acceleration);
            }
            set
            {
                xSpring.Acceleration = value.x;
                ySpring.Acceleration = value.y;
                zSpring.Acceleration = value.z;
            }
        }

        /// <summary>
        /// The target of the spring.
        /// </summary>
        public Vector3 Target
        {
            get
            {
                return new Vector3 (xSpring.Target, ySpring.Target, zSpring.Target);
            }
            set
            {
                xSpring.Target = value.x;
                ySpring.Target = value.y;
                zSpring.Target = value.z;
            }
        }

        /// <summary>
        /// The damper of the spring, a range from 0 to 1, with 0 having no damping (spring never stops overshooting), to 1 having max damping (no overshooting).
        /// </summary>
        public float Damper
        {
            get
            {
                return xSpring.Damper;
            }
            set
            {
                xSpring.Damper = value;
                ySpring.Damper = value;
                zSpring.Damper = value;
            }
        }
        /// <summary>
        /// The speed of the spring.
        /// </summary>
        public float Speed
        {
            get
            {
                return xSpring.Speed;
            }
            set
            {
                xSpring.Speed = value;
                ySpring.Speed = value;
                zSpring.Speed = value;
            }
        }

        /// <summary>
        /// Pushes the spring with the input acceleration.
        /// </summary>
        public void Accelerate (Vector3 acceleration)
        {
            xSpring.Accelerate (acceleration.x);
            ySpring.Accelerate (acceleration.y);
            zSpring.Accelerate (acceleration.z);
        }
    }
}
