using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "2D/Tiles/Extended Rule Tile")]
public class ExtendedRuleTile : RuleTile<RuleTile.TilingRuleOutput.Neighbor>
{
    public TileBase[] additionalTiles;
    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch (neighbor) {
            case TilingRuleOutput.Neighbor.This: return tile == this || (additionalTiles != null && additionalTiles.Contains(tile));
            case TilingRuleOutput.Neighbor.NotThis: return tile != this && (additionalTiles != null && !additionalTiles.Contains(tile));
        }
        return base.RuleMatch(neighbor, tile);
    }
}