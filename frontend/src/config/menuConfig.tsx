import React from 'react'
import type { MenuProps } from 'antd'
import {
  UserOutlined,
  ShopOutlined,
  ApartmentOutlined,
  CalendarOutlined,
  FileProtectOutlined,
  FolderOpenOutlined,
  ToolOutlined,
  TableOutlined,
  IdcardOutlined,
  GlobalOutlined,
  SolutionOutlined,
  FileTextOutlined,
  SettingOutlined,
  QuestionCircleOutlined,
} from '@ant-design/icons'

export interface MenuItemConfig {
  id: string
  parentId: string | null
  labelKey: string
  icon?: string
  route?: string
  ordinal: number
}

type MenuItem = Required<MenuProps>['items'][number]

const iconMap: Record<string, React.ComponentType> = {
  'osobno':                    UserOutlined,
  'poslovanje':                ShopOutlined,
  'poslovanje-org':            ApartmentOutlined,
  'poslovanje-godisnji':       CalendarOutlined,
  'poslovanje-ugovori-uprav':  FileProtectOutlined,
  'poslovanje-dokumenti-uprav':FolderOpenOutlined,
  'poslovanje-zaduzenja':      ToolOutlined,
  'sifarnici':                 TableOutlined,
  'sifarnici-osnovni':         IdcardOutlined,
  'sifarnici-adrese':          GlobalOutlined,
  'sifarnici-zaposlenje':      SolutionOutlined,
  'sifarnici-godisnji':        CalendarOutlined,
  'sifarnici-dokumenti':       FileTextOutlined,
  'sifarnici-organizacija':    ApartmentOutlined,
  'administracija':            SettingOutlined,
  'upute':                     QuestionCircleOutlined,
}

export const menuConfig: MenuItemConfig[] = [
  // ── Osobno ──────────────────────────────────────────────────────────────
  { id: 'osobno',             parentId: null,     labelKey: 'menu.osobno-grupa',               icon: 'osobno',   ordinal: 1 },
  { id: 'osobno-moji-podaci', parentId: 'osobno', labelKey: 'menu.osobno.mojiPodaci',          route: '/osobno/moji-podaci',    ordinal: 1 },
  { id: 'osobno-godisnji',    parentId: 'osobno', labelKey: 'menu.osobno.mojiGodisnji',        route: '/osobno/godisnji-odmori', ordinal: 2 },

  // ── Poslovanje ───────────────────────────────────────────────────────────
  { id: 'poslovanje',               parentId: null,        labelKey: 'menu.poslovanje-grupa',                     icon: 'poslovanje', ordinal: 2 },
  { id: 'poslovanje-osobe',         parentId: 'poslovanje', labelKey: 'menu.poslovanje.podaciOOsobama',           route: '/poslovanje/podaci-o-osobama',    ordinal: 1 },
  { id: 'poslovanje-zaposlenici',   parentId: 'poslovanje', labelKey: 'menu.poslovanje.zaposlenici',              route: '/poslovanje/zaposlenici',          ordinal: 2 },
  { id: 'poslovanje-agencijski',    parentId: 'poslovanje', labelKey: 'menu.poslovanje.agencijskiRadnici',        route: '/poslovanje/agencijski-radnici',   ordinal: 3 },
  { id: 'poslovanje-studenti',      parentId: 'poslovanje', labelKey: 'menu.poslovanje.studentiUcenici',          route: '/poslovanje/studenti-ucenici',     ordinal: 4 },
  { id: 'poslovanje-ugovori-djelo', parentId: 'poslovanje', labelKey: 'menu.poslovanje.ugovoriODjelu',            route: '/poslovanje/ugovori-o-djelu',      ordinal: 5 },

  // Poslovanje → Organizacija
  { id: 'poslovanje-org',                 parentId: 'poslovanje',     labelKey: 'menu.poslovanje.organizacija-grupa',                   icon: 'poslovanje-org', ordinal: 6 },
  { id: 'poslovanje-org-hijerarhija',     parentId: 'poslovanje-org', labelKey: 'menu.poslovanje.organizacija.hijerarhija',             route: '/poslovanje/organizacija/hijerarhija',     ordinal: 1 },
  { id: 'poslovanje-org-katalog-poslova', parentId: 'poslovanje-org', labelKey: 'menu.poslovanje.organizacija.katalogPoslova',          route: '/poslovanje/organizacija/katalog-poslova', ordinal: 2 },
  { id: 'poslovanje-org-radna-mjesta',    parentId: 'poslovanje-org', labelKey: 'menu.poslovanje.organizacija.radnaMjesta',             route: '/poslovanje/organizacija/radna-mjesta',    ordinal: 3 },

  // Poslovanje → Godišnji odmori
  { id: 'poslovanje-godisnji',           parentId: 'poslovanje',          labelKey: 'menu.poslovanje.godisnjiOdmori-grupa',                   icon: 'poslovanje-godisnji', ordinal: 7 },
  { id: 'poslovanje-godisnji-parametri', parentId: 'poslovanje-godisnji', labelKey: 'menu.poslovanje.godisnjiOdmori.parametriZaObracun',      route: '/poslovanje/godisnji-odmori/parametri', ordinal: 1 },
  { id: 'poslovanje-godisnji-kalendar',  parentId: 'poslovanje-godisnji', labelKey: 'menu.poslovanje.godisnjiOdmori.kalendarPrisutnosti',     route: '/poslovanje/godisnji-odmori/kalendar',  ordinal: 2 },

  // Poslovanje → Upravljanje ugovorima
  { id: 'poslovanje-ugovori-uprav',   parentId: 'poslovanje',              labelKey: 'menu.poslovanje.upravljanjeUgovorima-grupa',              icon: 'poslovanje-ugovori-uprav', ordinal: 8 },
  { id: 'poslovanje-ugovori-masovno', parentId: 'poslovanje-ugovori-uprav', labelKey: 'menu.poslovanje.upravljanjeUgovorima.masovnoProduzenje', route: '/poslovanje/upravljanje-ugovorima/masovno-produzenje', ordinal: 1 },

  // Poslovanje → Upravljanje dokumentima
  { id: 'poslovanje-dokumenti-uprav',   parentId: 'poslovanje',                   labelKey: 'menu.poslovanje.upravljanjeDokumentima-grupa',              icon: 'poslovanje-dokumenti-uprav', ordinal: 9 },
  { id: 'poslovanje-dokumenti-katalog', parentId: 'poslovanje-dokumenti-uprav',   labelKey: 'menu.poslovanje.upravljanjeDokumentima.katalogDokumenata',  route: '/poslovanje/upravljanje-dokumentima/katalog', ordinal: 1 },

  // Poslovanje → Upravljanje zaduženjima
  { id: 'poslovanje-zaduzenja',         parentId: 'poslovanje',             labelKey: 'menu.poslovanje.upravljanjeZaduzenjima-grupa',             icon: 'poslovanje-zaduzenja', ordinal: 10 },
  { id: 'poslovanje-zaduzenja-katalog', parentId: 'poslovanje-zaduzenja',   labelKey: 'menu.poslovanje.upravljanjeZaduzenjima.katalogOpreme',     route: '/poslovanje/upravljanje-zaduzenjima/katalog-opreme', ordinal: 1 },

  // ── Šifarnici ────────────────────────────────────────────────────────────
  { id: 'sifarnici', parentId: null, labelKey: 'menu.sifarnici-grupa', icon: 'sifarnici', ordinal: 3 },

  // Šifarnici → Osnovni
  { id: 'sifarnici-osnovni',         parentId: 'sifarnici',         labelKey: 'menu.sifarnici.osnovni-grupa',                  icon: 'sifarnici-osnovni', ordinal: 1 },
  { id: 'sifarnici-osnovni-spolovi', parentId: 'sifarnici-osnovni', labelKey: 'menu.sifarnici.osnovni.spolovi',  route: '/sifarnici/spolovi', ordinal: 1 },

  // Šifarnici → Adrese
  { id: 'sifarnici-adrese',          parentId: 'sifarnici',        labelKey: 'menu.sifarnici.adrese-grupa',                   icon: 'sifarnici-adrese', ordinal: 2 },
  { id: 'sifarnici-adrese-drzave',   parentId: 'sifarnici-adrese', labelKey: 'menu.sifarnici.adrese.drzave',   route: '/sifarnici/adrese/drzave',   ordinal: 1 },
  { id: 'sifarnici-adrese-zupanije', parentId: 'sifarnici-adrese', labelKey: 'menu.sifarnici.adrese.zupanije', route: '/sifarnici/adrese/zupanije', ordinal: 2 },
  { id: 'sifarnici-adrese-opcine',   parentId: 'sifarnici-adrese', labelKey: 'menu.sifarnici.adrese.opcine',   route: '/sifarnici/adrese/opcine',   ordinal: 3 },
  { id: 'sifarnici-adrese-naselja',  parentId: 'sifarnici-adrese', labelKey: 'menu.sifarnici.adrese.naselja',  route: '/sifarnici/adrese/naselja',  ordinal: 4 },

  // Šifarnici → Zaposlenje
  { id: 'sifarnici-zaposlenje',        parentId: 'sifarnici',            labelKey: 'menu.sifarnici.zaposlenje-grupa',                    icon: 'sifarnici-zaposlenje', ordinal: 3 },
  { id: 'sifarnici-zaposlenje-tipovi', parentId: 'sifarnici-zaposlenje', labelKey: 'menu.sifarnici.zaposlenje.tipoviRadnogOdnosa',        route: '/sifarnici/zaposlenje/tipovi-radnog-odnosa', ordinal: 1 },

  // Šifarnici → Godišnji odmori
  { id: 'sifarnici-godisnji',          parentId: 'sifarnici',           labelKey: 'menu.sifarnici.godisnjiOdmori-grupa',                 icon: 'sifarnici-godisnji', ordinal: 4 },
  { id: 'sifarnici-godisnji-praznici', parentId: 'sifarnici-godisnji',  labelKey: 'menu.sifarnici.godisnjiOdmori.praznici',              route: '/sifarnici/godisnji-odmori/praznici', ordinal: 1 },

  // Šifarnici → Dokumenti
  { id: 'sifarnici-dokumenti',        parentId: 'sifarnici',            labelKey: 'menu.sifarnici.dokumenti-grupa',                     icon: 'sifarnici-dokumenti', ordinal: 5 },
  { id: 'sifarnici-dokumenti-tipovi', parentId: 'sifarnici-dokumenti',  labelKey: 'menu.sifarnici.dokumenti.tipoviDokumenata',           route: '/sifarnici/dokumenti/tipovi', ordinal: 1 },

  // Šifarnici → Organizacija
  { id: 'sifarnici-organizacija',        parentId: 'sifarnici',               labelKey: 'menu.sifarnici.organizacija-grupa',                         icon: 'sifarnici-organizacija', ordinal: 6 },
  { id: 'sifarnici-organizacija-vrste',  parentId: 'sifarnici-organizacija',  labelKey: 'menu.sifarnici.organizacija.vrsteOrganizacijskihJedinica',   route: '/sifarnici/organizacija/vrste-jedinica', ordinal: 1 },

  // ── Top-level listovi ────────────────────────────────────────────────────
  { id: 'administracija', parentId: null, labelKey: 'menu.administracija', icon: 'administracija', route: '/administracija', ordinal: 4 },
  { id: 'upute',          parentId: null, labelKey: 'menu.upute',          icon: 'upute',          route: '/upute',          ordinal: 5 },
]

export function buildMenuTree(
  items: MenuItemConfig[],
  t: (key: string) => string,
  parentId: string | null = null,
): MenuItem[] {
  return items
    .filter(item => item.parentId === parentId)
    .sort((a, b) => a.ordinal - b.ordinal)
    .map(item => {
      const children = buildMenuTree(items, t, item.id)
      const IconComp = item.icon ? iconMap[item.icon] : undefined
      return {
        key: item.route ?? item.id,
        label: t(item.labelKey),
        icon: IconComp ? <IconComp /> : undefined,
        children: children.length > 0 ? children : undefined,
      }
    })
}
