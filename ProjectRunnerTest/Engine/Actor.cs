using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MonoEditorEndless.Engine
{
    internal class Actor
    {
        Vector3 _position;
        float _velocity;
        Vector3 _forwardVector;
        Model _model;

        public Actor()
        {
            _forwardVector = -Vector3.UnitZ;
            _position = Vector3.Zero;
            _velocity = 0f;
        }
        public Actor(Vector3 position)
        {
            _position = position;
        }
        // Getters
        public Vector3 GetPosition() { return _position; }
        public Vector3 GetForward() { return _forwardVector; }
        // Setters
        public void SetVelocity(float velocity) { _velocity = velocity; }
        public void SetForward(Vector3 forwardVector) { _forwardVector = forwardVector; }
        public void LoadModel(Model model)
        {
            _model = model;
        }
        public void Update(GameTime gameTime)
        {
            _position += _forwardVector * _velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
