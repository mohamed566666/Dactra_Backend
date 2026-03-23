namespace Dactra.DTOs.PaymobDto
{
    public class PaymobCallbackRequest
    {
        public string type { get; set; } = string.Empty;
        public PaymobCallbackObj? obj { get; set; }
    }

    public class PaymobCallbackObj
    {
        public int id { get; set; }
        public long amount_cents { get; set; }
        public string currency { get; set; } = string.Empty;
        public bool success { get; set; }
        public bool pending { get; set; }
        public bool is_refunded { get; set; }
        public int integration_id { get; set; }
        public DateTime created_at { get; set; }
        public bool error_occured { get; set; }
        public bool has_parent_transaction { get; set; }
        public bool is_3d_secure { get; set; }
        public bool is_auth { get; set; }
        public bool is_capture { get; set; }
        public bool is_standalone_payment { get; set; }
        public bool is_voided { get; set; }
        public int owner { get; set; }

        public PaymobCallbackOrder? order { get; set; }
        public PaymobSourceData? source_data { get; set; }
    }

    public class PaymobCallbackOrder
    {
        public int id { get; set; }
        public string? merchant_order_id { get; set; }
    }

    public class PaymobSourceData
    {
        public string? type { get; set; }
        public string? sub_type { get; set; }
        public string? pan { get; set; }
    }

}
