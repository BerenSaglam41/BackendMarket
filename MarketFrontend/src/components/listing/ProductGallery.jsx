export default function ProductGallery({ image }) {
  return (
    <div className="bg-white rounded-xl shadow-md border border-gray-100 p-4 sticky top-24">
      {/* Büyük Görsel */}
      <div className="aspect-square relative flex items-center justify-center overflow-hidden rounded-lg bg-white">
        <img
          src={image}
          alt="Ürün"
          className="object-contain w-full h-full hover:scale-105 transition-transform duration-500 cursor-zoom-in"
        />
      </div>
      
      {/* Thumbnail Alanı (İleride çoklu resim gelirse burası dolar) */}
      <div className="mt-4 flex gap-2 overflow-x-auto pb-2">
        <div className="w-16 h-16 border-2 border-orange-500 rounded-md p-1 cursor-pointer">
           <img src={image} className="w-full h-full object-contain" />
        </div>
        {/* Diğer resimler buraya map edilecek */}
      </div>
    </div>
  );
}