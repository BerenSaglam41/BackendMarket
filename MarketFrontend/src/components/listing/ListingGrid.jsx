import ListingCard from "./ListingCard";

export default function ListingGrid({ listings }) {
  return (
<div className="
  grid 
  grid-cols-1 
  sm:grid-cols-2 
  md:grid-cols-3 
  lg:grid-cols-4 
  gap-4
  justify-items-center
  md:justify-items-stretch
">
      {listings.map((item) => (
        <ListingCard key={item.listingId} listing={item} />
      ))}
    </div>
  );
}