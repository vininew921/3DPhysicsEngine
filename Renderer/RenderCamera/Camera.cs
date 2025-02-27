﻿using GLFW;
using GlmNet;
using Renderer.Shaders;
using static OpenGL.GL;

namespace Renderer.RenderCamera
{
    public class Camera
    {
        public vec3 Position;
        public vec3 Orientation = new vec3(0, 0, -1);
        public vec3 Up = new vec3(0, 1, 0);
        public mat4 CameraMatrix = new mat4(1);

        public int Width;
        public int Height;

        public float speed = 0.1f;
        public float sensitivity = 100;

        public bool firstClick = true;

        public Camera(int width, int height, vec3 position)
        {
            Width = width;
            Height = height;
            Position = position;
        }

        public void UpdateMatrix(float FOVdeg, float nearPlane, float farPlane)
        {
            mat4 view = glm.lookAt(Position, Position + Orientation, Up);
            mat4 projection = glm.perspective(glm.radians(FOVdeg), Width / Height, nearPlane, farPlane);

            CameraMatrix = projection * view;
        }

        public void Matrix(Shader shader, string uniform) => glUniformMatrix4fv(glGetUniformLocation(shader.ID, uniform), 1, false, (CameraMatrix).to_array());

        public void Inputs(Window window)
        {
            if (Glfw.GetKey(window, Keys.W) == InputState.Press)
            {
                Position += speed * Orientation;
            }

            if (Glfw.GetKey(window, Keys.A) == InputState.Press)
            {
                Position += speed * glm.normalize(glm.cross(Orientation, Up)) * -1;
            }

            if (Glfw.GetKey(window, Keys.S) == InputState.Press)
            {
                Position += speed * Orientation * -1;
            }

            if (Glfw.GetKey(window, Keys.D) == InputState.Press)
            {
                Position += speed * glm.normalize(glm.cross(Orientation, Up));
            }

            if (Glfw.GetKey(window, Keys.Space) == InputState.Press)
            {
                Position += speed * Up;
            }

            if (Glfw.GetKey(window, Keys.LeftControl) == InputState.Press)
            {
                Position += speed * Up * -1;
            }

            if (Glfw.GetKey(window, Keys.LeftShift) == InputState.Press)
            {
                speed = 0.4f;
            }

            if (Glfw.GetKey(window, Keys.LeftShift) == InputState.Release)
            {
                speed = 0.1f;
            }

            if (Glfw.GetMouseButton(window, MouseButton.Left) == InputState.Press)
            {
                if (firstClick)
                {
                    firstClick = false;
                    Glfw.SetCursorPosition(window, Width / 2, Height / 2);
                }

                Glfw.SetInputMode(window, InputMode.Cursor, (int)CursorMode.Hidden);
                Glfw.GetCursorPosition(window, out double mouseX, out double mouseY);

                float rotX = sensitivity * (float)(mouseY - (Height / 2)) / Height;
                float rotY = sensitivity * (float)(mouseX - (Height / 2)) / Height;

                vec3 newOrientation = glm.rotate(Orientation, glm.radians(-rotX), glm.normalize(glm.cross(Orientation, Up)));
                if (!((glm.angle(newOrientation, Up) <= glm.radians(5)) || (glm.angle(newOrientation, Up * -1) <= glm.radians(5))))
                {
                    Orientation = newOrientation;
                }

                Orientation = glm.rotate(Orientation, glm.radians(-rotY), Up);

                Glfw.SetCursorPosition(window, Width / 2, Height / 2);
            }
            else if (Glfw.GetMouseButton(window, MouseButton.Left) == InputState.Release)
            {
                Glfw.SetInputMode(window, InputMode.Cursor, (int)CursorMode.Normal);
                firstClick = true;
            }
        }
    }
}
