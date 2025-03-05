using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("api/delivery")]
 public class DeliverController:ControllerBase {
    private static readonly Dictionary<string,string> ProductLocations = new Dictionary<string, string>{
        {"A","C1"},
        {"B","C1"},
        {"C","C1"},
        {"D","C2"},
        {"E","C2"},
        {"F","C2"},
           {"G","C3"},
        {"H","C3"},
        {"I","C3"}
    };

     private static readonly Dictionary<string,double> ProductWeights = new Dictionary<string, double>{
        {"A",2},
        {"B",3},
        {"C",8},
         {"D",12},
        {"E",25},
        {"F",15},
         {"G",0.5},
        {"H",1},
        {"I",2},
        
    };
        private static readonly Dictionary<string,double> CenterToDestination = new Dictionary<string, double>{
        {"C1",3},
        {"C2",2.5},
        {"C3",2},
        
    };
        private static readonly Dictionary<(string, string), int> CenterToCenterDistances = new()
        {
            { ("C1", "C2"), 10 }, { ("C1", "C3"), 15 }, { ("C2", "C3"), 12 }
        };


  private const int TransferCost = 5; 
    private const int additionalCost=8;


[HttpPost("calculate_cost")]

          public IActionResult CalculateDeliveryCost([FromBody] OrderRequest request)
        {
            if (request?.Order == null || request.Order.Count == 0)
                return BadRequest(new { Error = "Invalid order request." });

            double minCost = CalculateMinCost(request.Order);
            return Ok(new { TotalCost = minCost });
        }
private double CalculateMinCost(Dictionary<string, int> order)
        {
            Dictionary<string, double> centerWeights = new();

            foreach (var item in order)
            {
                string center = ProductLocations[item.Key];
                double weight = ProductWeights[item.Key] * item.Value;

                if (centerWeights.ContainsKey(center))
                    centerWeights[center] += weight;
                else
                    centerWeights[center] = weight;
            }

            double minCost = int.MaxValue;

            foreach (var startCenter in centerWeights.Keys)
            {
                double totalCost = 0;

                foreach (var center in centerWeights.Keys)
                {
                    double weight = centerWeights[center];

                    double costDirect = CalculateDistanceCost(CenterToDestination[center], weight);
                    double costWithTransfer = int.MaxValue;

                    foreach (var intermediate in centerWeights.Keys)
                    {
                        if (center == intermediate) continue;

                        if (CenterToCenterDistances.TryGetValue((center, intermediate), out int centerDistance) ||
                            CenterToCenterDistances.TryGetValue((intermediate, center), out centerDistance))
                        {
                            double transferCost = TransferCost * weight;
                            double costThroughIntermediate = CalculateDistanceCost(centerDistance, weight) + transferCost +
                                                          CalculateDistanceCost(CenterToDestination[intermediate], weight);
                            costWithTransfer = Math.Min(costWithTransfer, costThroughIntermediate);
                        }
                    }

                    totalCost += Math.Min(costDirect, costWithTransfer);
                }

                minCost = Math.Min(minCost, totalCost);
            }

            return minCost;
        }


private double CalculateDistanceCost(double distance,double weight)
{

    double costPerKm=(weight<=5? 10:(5*10)+(weight-5)/5*8)+((weight-5)/5*8)+((weight-5)==0?0:8);
    return costPerKm*distance;
}

    
 private int CalculateCostPerKm(int weight)
 {
    int cost=0;
    if(weight<=5){
        cost=10;
    }
    else
    {
        cost=(5*10)*((weight-5)/5*8);
        if((weight-5)%5 != 0)
        {
            cost+=8;
        }
    }
 return cost/weight;
    
 }

}