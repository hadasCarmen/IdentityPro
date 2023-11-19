using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace IdentityPro.Models
{
	public class Order
	{
		public int Id { get; set; }
		public List<OrderItem> Products { get; set; }
		[DataType(DataType.Date)]
		public DateTime? CreatedDate { get; set; }
		public DateTime? DeliveryDate { get; set; }
		public  Weather Weather { get; set; }
    }
 //   public class Weather
	//{
	//	public string Season { get; set; }
	//	public int Temperature { get; set; }
	//	public int Humidity { get; set; }
	//}

}
