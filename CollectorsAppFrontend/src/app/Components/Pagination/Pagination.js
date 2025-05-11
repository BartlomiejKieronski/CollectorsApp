import { useEffect } from 'react';
import './Pagination.css';
import Image from 'next/image';
export default function Pagination ({ totalItems, itemsPerPage,currentPage,onPageChange }) {
  const totalPages = Math.ceil(totalItems / itemsPerPage);

  useEffect(() => {
    if (onPageChange) {
      onPageChange(currentPage);
    }
  }, [currentPage, onPageChange]);

  const goToPage = (page) => {
    if (page < 1 || page > totalPages) return;
    onPageChange(page);
  };

  return (
    <div className="pagination">
      <button
        className="pagination-button icon"
        onClick={() => goToPage(1)}
        disabled={currentPage === 1}
      >
        <Image src={"/first.svg"} alt='First page' width={28} height={28}/>
      </button>
      <button
        className="pagination-button icon"
        onClick={() => goToPage(currentPage - 1)}
        disabled={currentPage === 1}
      >
        <Image src={"/previous.svg"} height={28} width={28} alt='Previous page' />
      </button>
      {currentPage!==1 && (<button
        className="pagination-button"
        onClick={() => goToPage(1)}
        disabled={currentPage === 1}
      >
        1
      </button>)}
      {currentPage>4 && (<button
        className="pagination-button"
        disabled
      >
        ...
      </button>)}
      {currentPage==4 && (<button
        className="pagination-button"
        onClick={() => goToPage(2)}
      >
        2
      </button>)}
      {(currentPage!==1 && (currentPage) !==2) && (<button
        className="pagination-button"
        onClick={() => goToPage(currentPage - 1)}
      >
        {currentPage-1}
      </button>)}
      <button className="pagination-button active-page" disabled>
        {currentPage}
      </button>
      {currentPage < totalPages - 1 && (
        <button
          className="pagination-button"
          onClick={() => goToPage(currentPage + 1)}
        >
          {currentPage + 1}
        </button>
      )}
      {(totalPages-3)>currentPage && (
        <button
          className="pagination-button"
          disabled
        >
          ...
        </button>
      )}
        {(totalPages-1)==currentPage+2 && (
        <button
          className="pagination-button"
          onClick={() => goToPage(totalPages-1)}
        >
          {totalPages-1}
        </button>
      )}
      {!(currentPage == totalPages) && (
        <button
          className="pagination-button"
          onClick={() => goToPage(totalPages)}
        >
          {totalPages}
        </button>
      )}
      <button
        className="pagination-button icon"
        onClick={() => goToPage(currentPage + 1)}
        disabled={currentPage === totalPages}
      >
        <Image src={"/next.svg"} height={28} width={28} alt='Next page'/>
      </button>
      <button
        className="pagination-button icon"
        onClick={() => goToPage(totalPages)}
        disabled={currentPage === totalPages}
      >
        <Image src={"/last.svg"} height={28} width={28} alt='Last page' />
      </button>
    </div>
  );
};

