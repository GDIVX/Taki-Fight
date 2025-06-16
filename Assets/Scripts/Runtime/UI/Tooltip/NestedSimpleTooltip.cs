namespace Runtime.UI.Tooltip
{
    public class NestedSimpleTooltip : SimpleTooltipController
    {
        protected override void CalculatePosition()
        {
            //do nothing
            //this is a nested tooltip and the parent will handle it
        }
    }
}