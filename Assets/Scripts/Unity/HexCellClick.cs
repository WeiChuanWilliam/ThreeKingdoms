using ThreeKindoms.Core;
using UnityEngine;

namespace ThreeKindoms.UnityBridge
{
    [RequireComponent(typeof(Collider2D))]
    public class HexCellClick : MonoBehaviour
    {
        HexCoord _coord;
        HexSpikeBootstrap _bootstrap;

        public void Init(HexCoord coord, HexSpikeBootstrap bootstrap)
        {
            _coord = coord;
            _bootstrap = bootstrap;
            var col = GetComponent<Collider2D>();
            if (col == null)
            {
                col = gameObject.AddComponent<CircleCollider2D>();
                ((CircleCollider2D)col).radius = 0.45f;
            }
        }

        void OnMouseDown()
        {
            bool preview = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            _bootstrap.OnCellClicked(_coord, preview);
        }

        void OnMouseEnter() => _bootstrap.OnCellHover(_coord);
    }
}
