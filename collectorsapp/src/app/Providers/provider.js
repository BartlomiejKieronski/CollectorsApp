'use client';
import { ThemeProvider } from 'next-themes';

export default function Providers({ children }) {

  return (
    <ThemeProvider themes={["dark","light"]} value={{dark:"dark",light:"light"}} defaultTheme='dark' attribute="class">
      {children}
    </ThemeProvider>
  );
}