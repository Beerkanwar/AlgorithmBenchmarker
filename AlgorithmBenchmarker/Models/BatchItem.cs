using CommunityToolkit.Mvvm.ComponentModel;

namespace AlgorithmBenchmarker.Models
{
    public class BatchItem : ObservableObject
    {
        private string _batchId = string.Empty;
        public string BatchId
        {
            get => _batchId;
            set => SetProperty(ref _batchId, value);
        }

        private string _algorithmName = string.Empty;
        public string AlgorithmName
        {
            get => _algorithmName;
            set => SetProperty(ref _algorithmName, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public override string ToString()
        {
            return AlgorithmName;
        }
    }
}
