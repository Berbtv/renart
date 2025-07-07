import React, { useState, useEffect } from 'react';
import './ProductList.css';

function buildQuery(filters) {
  const params = new URLSearchParams();
  if (filters.minPrice) params.append('minPrice', filters.minPrice);
  if (filters.maxPrice) params.append('maxPrice', filters.maxPrice);
  if (filters.minPopularity) params.append('minPopularity', filters.minPopularity);
  if (filters.maxPopularity) params.append('maxPopularity', filters.maxPopularity);
  return params.toString();
}

const colorKeys = {
  'Yellow Gold': 'yellow',
  'White Gold': 'white',
  'Rose Gold': 'rose'
};

function ProductList() {
  const [products, setProducts] = useState([]);
  const [filterOpen, setFilterOpen] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [selectedColors, setSelectedColors] = useState([]);
  // Yeni filtre state'leri
  const [minPrice, setMinPrice] = useState('');
  const [maxPrice, setMaxPrice] = useState('');
  const [minPopularity, setMinPopularity] = useState('');
  const [maxPopularity, setMaxPopularity] = useState('');

  // API'den ürünleri çek
  const fetchProducts = async (filters = {}) => {
    setLoading(true);
    setError(null);
    try {
      const query = buildQuery(filters);
      const response = await fetch(`/api/Products/filter?${query}`);
      if (!response.ok) throw new Error('API hatası');
      const data = await response.json();
      setProducts(data.data || data); // API'den ApiResponse ile gelirse .data kullan
      setSelectedColors((data.data || data).map(() => 'Yellow Gold'));
    } catch (err) {
      setError('Ürünler yüklenemedi.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchProducts();
    // eslint-disable-next-line
  }, []);

  // Filtreyi uygula butonu
  const handleApplyFilter = () => {
    fetchProducts({
      minPrice: minPrice || undefined,
      maxPrice: maxPrice || undefined,
      minPopularity: minPopularity ? ((minPopularity / 5 ) ) : undefined,
      maxPopularity: maxPopularity ? ((maxPopularity / 5 )) : undefined
    });
    setFilterOpen(false);
  };

  // Filtreleri temizle
  const handleClearFilters = () => {
    setMinPrice('');
    setMaxPrice('');
    setMinPopularity('');
    setMaxPopularity('');
    fetchProducts({});
  };

  // Slider ileri/geri fonksiyonları
  const scrollSlider = (direction) => {
    const slider = document.querySelector('.product-slider');
    if (slider) {
      const card = slider.querySelector('.product-card');
      const cardWidth = card ? card.offsetWidth + 32 : 320; // 32px gap
      const scrollAmount = cardWidth * 4; // 4 kart birden kaydır
      slider.scrollBy({ left: direction === 'left' ? -scrollAmount : scrollAmount, behavior: 'smooth' });
    }
  };

  return (
    <div className="container">
      <h1 className="title">Product List</h1>
      <button className="filter-btn" onClick={() => setFilterOpen(true)}>Filtrele</button>
      <button className="slider-arrow left edge" onClick={() => scrollSlider('left')}>&lt;</button>
      <button className="slider-arrow right edge" onClick={() => scrollSlider('right')}>&gt;</button>
      {loading && <div>Yükleniyor...</div>}
      {error && <div style={{color:'red'}}>{error}</div>}
      <div className="product-slider">
        {products.map((product, idx) => {
          // 0.5 hassasiyetli rating hesaplama
          const roundedScore = Math.round(product.popularityScore * 10) / 2;
          const fullStars = Math.floor(roundedScore);
          const halfStar = roundedScore % 1 !== 0;
          const emptyStars = 5 - Math.ceil(roundedScore);
          return (
            <div className="product-card card-transparent" key={idx}>
              <div className="product-img-bg">
                <img
                  src={product.images && product.images[colorKeys[selectedColors[idx]]]}
                  alt={product.name}
                  className="product-img"
                />
              </div>
              <div className="product-title">{product.name}</div>
              <div className="product-price">{product.priceFormatted || `$${product.price?.toFixed(2)}`} USD</div>
             <div className="color-options">
  {Object.entries(colorKeys).map(([colorName, colorKey]) => (
    <span
      key={colorKey}
      className="color-dot-svg"
      title={colorName}
      onClick={() => setSelectedColors(prev => prev.map((c, i) => i === idx ? colorName : c))}
      style={{ cursor: 'pointer', marginRight: 16 }}
    >
      <svg width="32" height="32" viewBox="0 0 100 100">
        {selectedColors[idx] === colorName && (
          <circle cx="50" cy="50" r="48" fill="none" stroke="#222" strokeWidth="4"/>
        )}
        <circle
          cx="50"
          cy="50"
          r="34"
          fill={
            colorKey === 'yellow' ? '#E6CA97'
            : colorKey === 'white' ? '#D9D9D9'
            : colorKey === 'rose' ? '#E1A4A9'
            : '#fff'
          }
        />
      </svg>
    </span>
  ))}
</div>
              <div className="color-name color-name-yellow">{selectedColors[idx]}</div>
              <div className="rating">
              <span className="rating-stars">
  {[...Array(fullStars)].map((_, i) => (
    <span key={i} className="star">&#9733;</span>
  ))}
  {halfStar && (
    <span key="half" className="star half" >
      <svg
        width="15"
        height="15"
        
        style={{
          
          verticalAlign: 'middle',
          fontSize: '1.3rem',
          transform: 'scale(0.70)'
        }}
      >
        <defs>
          <linearGradient id={`half-grad-${idx}`}>
            <stop offset="50%" stopColor="#E6CA97" />
            <stop offset="50%" stopColor="#D9D9D9" />
          </linearGradient>
        </defs>
        <polygon
          points="12,2 15,9 22,9 17,14 18,21 12,17 6,21 7,14 2,9 9,9"
          fill={`url(#half-grad-${idx})`}
          stroke="#E6CA97"
          strokeWidth="1"
        />
      </svg>
    </span>
  )}
  {[...Array(emptyStars)].map((_, i) => (
    <span key={i} className="star empty">&#9733;</span>
  ))}
</span>
                <span className="rating-value">{roundedScore}/5</span>
              </div>
            </div>
          );
        })}
      </div>
      {/* Filtre paneli sadece filterOpen true ise gösterilecek */}
      {filterOpen && (
        <div className="filter-panel open">
          <button className="close-filter" onClick={() => setFilterOpen(false)}>Kapat</button>
          <h2>Filtrele</h2>
          <div className="filter-group">
            <label>Minimum Fiyat ($):</label>
            <input type="number" className="form-control" value={minPrice} onChange={e => setMinPrice(e.target.value)} placeholder="0" />
          </div>
          <div className="filter-group">
            <label>Maksimum Fiyat ($):</label>
            <input type="number" className="form-control" value={maxPrice} onChange={e => setMaxPrice(e.target.value)} placeholder="1000" />
          </div>
          <div className="filter-group">
            <label>Minimum Popülerlik (1-5):</label>
            <input type="number" className="form-control" min="1" max="5" step="1" value={minPopularity} onChange={e => setMinPopularity(e.target.value)} placeholder="1" />
          </div>
          <div className="filter-group">
            <label>Maksimum Popülerlik (1-5):</label>
            <input type="number" className="form-control" min="1" max="5" step="1" value={maxPopularity} onChange={e => setMaxPopularity(e.target.value)} placeholder="5" />
          </div>
          <button className="apply-filter" onClick={handleApplyFilter}>Filtrele</button>
          <button className="apply-filter" style={{marginLeft: 10, background: '#eee', color: '#222'}} onClick={handleClearFilters}>Filtreleri Temizle</button>
        </div>
      )}
    </div>
  );
}

export default ProductList;