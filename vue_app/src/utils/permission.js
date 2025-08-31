import store from '@/store';

export function getCurrentRole(): string {
    return String(store.getters.getUserRole || store.getters.getUserData?.userType || '').trim();
}
export function isSuperadminRole(role?: string) {
    return String(role || '').trim().toLowerCase() === 'superadmin';
}

export const perms = {
    isSuperadmin(): boolean {
        return isSuperadminRole(getCurrentRole());
    },
    viewDetails(target: { userType?: string } | null | undefined): boolean {
        return perms.isSuperadmin() || !isSuperadminRole(target?.userType);
    },
    deleteUser(target: { idPerson?: number; userType?: string } | null | undefined): boolean {
        const me = Number(store.getters.getPersonId || store.getters.getUserData?.idPerson || 0);
        return !!target && !isSuperadminRole(target.userType) && Number(target.idPerson) !== me;
    },
    assignUserType(title: string): boolean {
        return !isSuperadminRole(title);
    },
};
