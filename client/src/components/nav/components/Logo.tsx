'use client';

import Image from 'next/image';
import { usePathname, useRouter } from 'next/navigation';

import logo from '../../../../public/blue_horizontal.png';

type Props = {
  size: number;
  className?: string;
};

export default function Logo({ className, size }: Props) {
  const router = useRouter();
  const pathname = usePathname();

  const doReset = () => {
    if (pathname !== '/') router.push('/');
  };

  return (
    <div
      onClick={doReset}
      className={`${className} cursor-pointer flex items-center gap-2 text-3xl font-semibold mx-5`}
    >
      <Image src={logo} height={size} width={size} priority alt="logo" />
    </div>
  );
}
