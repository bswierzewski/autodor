'use client';

import * as React from 'react';
import { Moon, Sun } from 'lucide-react';
import { useTheme } from 'next-themes';

import { Button } from '@/components/ui/button';
import { useEffect, useState } from 'react';

export function ThemePicker() {
  const { theme, setTheme } = useTheme();
  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    setMounted(true);
  }, []);

  return (
    mounted && (
      <Button size="icon" variant="ghost" onClick={() => setTheme(theme == 'dark' ? 'light' : 'dark')}>
        {theme == 'dark' ? <Sun className="h-[1.5rem] w-[1.5rem]" /> : <Moon className="h-[1.5rem] w-[1.5rem]" />}
      </Button>
    )
  );
}
