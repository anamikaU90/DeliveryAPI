using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("api/delivery")]
public class DeliverController:ControllerBase {
    private static readonly Dictionary<string,int> Distances = new Dictionary<string, int>{
        {"C1",10},
        {"C2",15},
        {"C3",20},
        
    };

    private const int costPerKm=2;
    private const int PickUpCost=5;


[HttpPost("calculate_cost")]
    public IActionResult CalculateCost([FromBody] OrderRequest order)
    {
       if(order?.Products==null || order.Products.Count==0)
       {
        return BadRequest(new{message="Invalid order data"});
       }
       int minCost= CalculateMinimumCost(order.Products);
       return Ok(new {minimum_delivery_cost=minCost});

    }

    private int CalculateMinimumCost(Dictionary<string,int>order)
    {
        string chosenWarehouse= Distances.OrderBy(d=>d.Value).First().Key;
        int baseCost=Distances[chosenWarehouse]*costPerKm;
        var requiredWarehouses=new HashSet<string>{"C1","C2","C3"};

        requiredWarehouses.Remove(chosenWarehouse);
        int pickUpCost=requiredWarehouses.Count*PickUpCost;
        return baseCost+pickUpCost;
    }
}