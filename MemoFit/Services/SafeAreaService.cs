namespace MemoFit.Services
{
    /// <summary>
    /// ネイティブ側のセーフエリア情報を Blazor 側に渡すサービス
    /// </summary>
    public class SafeAreaService
    {
        /// <summary>ステータスバーの高さ (px → CSS px に変換済み)</summary>
        public float StatusBarHeight { get; set; } = 0f;
    }
}
