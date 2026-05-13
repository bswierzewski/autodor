import { PackageIcon, UsersThreeIcon } from "@phosphor-icons/react";

const navItems = [
	{ label: "Zamówienia", to: "/", icon: PackageIcon },
	{ label: "Klienci", to: "/contractors", icon: UsersThreeIcon },
] as const;

export function Navbar() {
	return <div>Navbar</div>;
}