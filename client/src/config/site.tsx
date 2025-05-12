import { ScrollText, UserSearch } from 'lucide-react';

export type SiteConfig = typeof siteConfig;

export const siteConfig = {
  name: 'AUTODOR',
  description: 'Invoice clients',
  company: 'NinjaByte'
};

// Define button data
export const Urls = [
  {
    id: 1,
    label: 'Tablica',
    icon: <ScrollText />,
    iconSmall: <ScrollText className="h-4 w-4" />,
    route: '/dashboard'
  },
  {
    id: 2,
    label: 'Kontrahenci',
    icon: <UserSearch />,
    iconSmall: <UserSearch className="h-4 w-4" />,
    route: '/contractors'
  }
];
