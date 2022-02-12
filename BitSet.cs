public class BitSet
{
    private const int UINT32_SIZE = 32;
    
    private const int LOG2_UINT32_SIZE = 5;

    public BitSet(int length)
    {
        if(length < 0)
            throw new ArgumentOutOfRangeException();

        _bits = new uint[((length - 1) >> LOG2_UINT32_SIZE) + 1];
        
        Length = length;
    }

    private uint[] _bits;

    public int Length { get; private set; }
    
#region O(1)
    
    public bool Get(int key) => (_bits[key >> LOG2_UINT32_SIZE] & (1u << key)) != uint.MinValue;

    public void SetTrue(int key) => _bits[key >> LOG2_UINT32_SIZE] |= 1u << key;

    public void SetFalse(int key) => _bits[key >> LOG2_UINT32_SIZE] &= ~(1u << key);

    public void Set(int key, bool value)
    {
        if(value)
            SetTrue(key);
        else
            SetFalse(key);
    }

    public bool this[int key] { get => Get(key); set => Set(key, value); }
    
#endregion
    
#region O(n)
    
    /// <summary>
    /// Sets the bits in the given range from (inclusive) and to (exclusive) to true.
    /// </summary>
    public void SetTrue(int from, int to)
    {
        if(from >= to)
            return;

        int i = from >> LOG2_UINT32_SIZE;
        int length = to - 1 >> LOG2_UINT32_SIZE;

        if(i == length)
        {
            _bits[i] |= uint.MaxValue >> UINT32_SIZE - to & uint.MaxValue << from;

            return;
        }

        _bits[i] |= uint.MaxValue << from;
        _bits[length] |= uint.MaxValue >> UINT32_SIZE - to;

        for(i++; i < length; i++)
            _bits[i] = uint.MaxValue;
    }
        
    /// <summary>
    /// Sets the bits in the given range from (inclusive) and to (exclusive) to false.
    /// </summary>
    public void SetFalse(int from, int to)
    {
        if(from >= to)
            return;

        int i = from >> LOG2_UINT32_SIZE;
        int length = to - 1 >> LOG2_UINT32_SIZE;

        if(i == length)
        {
            _bits[i] &= ~(uint.MaxValue >> UINT32_SIZE - to & uint.MaxValue << from);

            return;
        }

        _bits[i] &= ~(uint.MaxValue << from);
        _bits[length] &= ~(uint.MaxValue >> UINT32_SIZE - to);

        for(i++; i < length; i++)
            _bits[i] = uint.MinValue;
    }
    
    /// <summary>
    /// Sets the bits in the given range from (inclusive) and to (exclusive) to the specified value.
    /// </summary>
    public void Set(int from, int to, bool value)
    {
        if(value)
            SetTrue(from, to);
        else
            SetFalse(from, to);
    }
    
    public void SetAll(bool value)
    {
        int length = _bits.Length;

        uint uintValue = value ? uint.MaxValue : uint.MinValue;

        for(int i = 0; i < length; i++)
            _bits[i] = uintValue;
    }
    
#endregion
}
