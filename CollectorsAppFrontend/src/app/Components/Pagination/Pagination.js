import { useEffect } from 'react';
import Style from './Pagination.module.css';
import Image from 'next/image';
import cn from 'classnames';

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
    <div className={cn(Style.pagination)}>
      <button
        className={cn(Style.paginationButton, 'icon')}
        onClick={() => goToPage(1)}
        disabled={currentPage === 1}
      >
        <Image src={"/first.svg"} alt='First page' width={28} height={28}/>
      </button>
      <button
        className={cn(Style.paginationButton, 'icon')}
        onClick={() => goToPage(currentPage - 1)}
        disabled={currentPage === 1}
      >
        <Image src={"/previous.svg"} height={28} width={28} alt='Previous page' />
      </button>
      {currentPage!==1 && (<button
        className={cn(Style.paginationButton)}
        onClick={() => goToPage(1)}
        disabled={currentPage === 1}
      >
        1
      </button>)}
      {currentPage>4 && (<button
        className={cn(Style.paginationButton)}
        disabled
      >
        ...
      </button>)}
      {currentPage==4 && (<button
        className={cn(Style.paginationButton)}
        onClick={() => goToPage(2)}
      >
        2
      </button>)}
      {(currentPage!==1 && (currentPage) !==2) && (<button
        className={cn(Style.paginationButton)}
        onClick={() => goToPage(currentPage - 1)}
      >
        {currentPage-1}
      </button>)}
      <button className={cn(Style.paginationButton, Style.activePage)} disabled>
        {currentPage}
      </button>
      {currentPage < totalPages - 1 && (
        <button
          className={cn(Style.paginationButton)}
          onClick={() => goToPage(currentPage + 1)}
        >
          {currentPage + 1}
        </button>
      )}
      {(totalPages-3)>currentPage && (
        <button
          className={cn(Style.paginationButton)}
          disabled
        >
          ...
        </button>
      )}
        {(totalPages-1)==currentPage+2 && (
        <button
          className={cn(Style.paginationButton)}
          onClick={() => goToPage(totalPages-1)}
        >
          {totalPages-1}
        </button>
      )}
      {!(currentPage == totalPages) && (
        <button
          className={cn(Style.paginationButton)}
          onClick={() => goToPage(totalPages)}
        >
          {totalPages}
        </button>
      )}
      <button
        className={cn(Style.paginationButton, 'icon')}
        onClick={() => goToPage(currentPage + 1)}
        disabled={currentPage === totalPages}
      >
        <Image src={"/next.svg"} height={28} width={28} alt='Next page'/>
      </button>
      <button
        className={cn(Style.paginationButton, 'icon')}
        onClick={() => goToPage(totalPages)}
        disabled={currentPage === totalPages}
      >
        <Image src={"/last.svg"} height={28} width={28} alt='Last page' />
      </button>
    </div>
  );
};

