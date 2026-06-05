import { useEffect, useRef, useMemo, useState } from 'react'
import { useTranslation } from 'react-i18next'
import {
  Button, Tag, Space, Form, Input, Select,
  InputNumber, Switch, App, Tooltip, Checkbox,
} from 'antd'
import AppModal from '@/components/AppModal'
import AgGridWrapper from '@/components/AgGridWrapper'
import { PlusOutlined, EditOutlined, StopOutlined, CheckOutlined, DeleteOutlined, DownloadOutlined } from '@ant-design/icons'
import type { ColDef, ICellRendererParams } from 'ag-grid-community'
import {
  countyApi, municipalityApi,
  type CountyResponse, type MunicipalityResponse, type CreateMunicipalityRequest,
} from '@/api/codebook'
import CodebookLayout from '@/layouts/CodebookLayout'

interface FormValues {
  code: string
  nameHr: string
  nameEn?: string | null
  countyId?: string | null
  ordinal: number
  isActive: boolean
}

export default function MunicipalityPage() {
  const { t } = useTranslation()
  const { message, modal } = App.useApp()

  const [data, setData]                 = useState<MunicipalityResponse[]>([])
  const [counties, setCounties]         = useState<CountyResponse[]>([])
  const [loading, setLoading]           = useState(false)
  const [onlyActive, setOnlyActive]     = useState(true)
  const [countyFilter, setCountyFilter] = useState<string | null>(null)
  const [modalOpen, setModalOpen]       = useState(false)
  const [saving, setSaving]             = useState(false)
  const [editingItem, setEditingItem]   = useState<MunicipalityResponse | null>(null)
  const [form] = Form.useForm<FormValues>()
  const exportRef = useRef<(() => void) | null>(null)

  const fetchCounties = async () => {
    try {
      const res = await countyApi.getAll(false)
      setCounties(res.data)
    } catch { /* silent */ }
  }

  const fetchData = async () => {
    setLoading(true)
    try {
      const res = await municipalityApi.getAll(true, countyFilter)
      setData(res.data)
    } catch {
      message.error('Greška pri dohvatu podataka')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => { fetchCounties() }, [])
  useEffect(() => { fetchData() }, [countyFilter])

  const filteredData = onlyActive ? data.filter(d => d.isActive) : data

  const openAddModal = () => {
    setEditingItem(null)
    form.resetFields()
    form.setFieldsValue({ isActive: true, ordinal: data.length + 1 })
    setModalOpen(true)
  }

  const openEditModal = (item: MunicipalityResponse) => {
    setEditingItem(item)
    form.setFieldsValue({
      code: item.code,
      nameHr: item.nameHr,
      nameEn: item.nameEn,
      countyId: item.countyId,
      ordinal: item.ordinal,
      isActive: item.isActive,
    })
    setModalOpen(true)
  }

  const handleSave = async () => {
    const values = await form.validateFields()
    setSaving(true)
    try {
      if (editingItem) {
        await municipalityApi.update(editingItem.id, values)
        message.success('Zapis ažuriran')
      } else {
        await municipalityApi.create(values as CreateMunicipalityRequest)
        message.success('Zapis dodan')
      }
      setModalOpen(false)
      fetchData()
    } catch {
      message.error('Greška pri spremanju')
    } finally {
      setSaving(false)
    }
  }

  const handleToggleActive = async (item: MunicipalityResponse) => {
    try {
      await municipalityApi.toggleActive(item.id)
      message.success(item.isActive ? 'Zapis deaktiviran' : 'Zapis aktiviran')
      fetchData()
    } catch {
      message.error('Greška pri promjeni statusa')
    }
  }

  const handleDelete = async (item: MunicipalityResponse) => {
    try {
      await municipalityApi.deleteById(item.id)
      message.success('Zapis obrisan')
      fetchData()
    } catch (err: unknown) {
      const status = (err as { response?: { status?: number } })?.response?.status
      message.error(status === 409 ? t('common.error_in_use') : t('common.error_delete'))
    }
  }

  const confirmToggleActive = (item: MunicipalityResponse) => {
    modal.confirm({
      title: t(item.isActive ? 'codebook.municipality.confirm.deactivate' : 'codebook.municipality.confirm.activate'),
      onOk: () => handleToggleActive(item),
      okText: t('common.yes'),
      cancelText: t('common.no'),
      centered: true,
    })
  }

  const confirmDelete = (item: MunicipalityResponse) => {
    modal.confirm({
      title: t('codebook.municipality.confirm.delete'),
      content: t('codebook.municipality.confirm.delete_description'),
      onOk: () => handleDelete(item),
      okText: t('codebook.municipality.confirm.delete_ok'),
      okType: 'danger',
      cancelText: t('common.no'),
      centered: true,
    })
  }

  const countyOptions = useMemo(
    () => counties.map(c => ({ value: c.id, label: c.nameHr })),
    [counties]
  )

  const columnDefs = useMemo<ColDef<MunicipalityResponse>[]>(() => [
    {
      field: 'code',
      headerName: t('codebook.municipality.columns.code'),
      width: 80,
    },
    {
      field: 'nameHr',
      headerName: t('codebook.municipality.columns.nameHr'),
      flex: 1,
    },
    {
      field: 'nameEn',
      headerName: t('codebook.municipality.columns.nameEn'),
      flex: 1,
      valueFormatter: (p) => p.value ?? '—',
    },
    {
      field: 'countyNameHr',
      headerName: t('codebook.municipality.columns.county'),
      flex: 1,
      valueFormatter: (p) => p.value ?? '—',
    },
    {
      field: 'ordinal',
      headerName: t('codebook.municipality.columns.ordinal'),
      width: 120,
      sort: 'asc',
    },
    {
      field: 'isActive',
      headerName: t('codebook.municipality.columns.status'),
      width: 120,
      filter: false,
      cellRenderer: (p: ICellRendererParams<MunicipalityResponse>) =>
        p.value
          ? <Tag color="success">{t('codebook.municipality.status.active')}</Tag>
          : <Tag color="default">{t('codebook.municipality.status.inactive')}</Tag>,
    },
    {
      headerName: t('codebook.municipality.columns.actions'),
      width: 120,
      sortable: false,
      filter: false,
      resizable: false,
      cellRenderer: (p: ICellRendererParams<MunicipalityResponse>) => {
        const rec = p.data!
        return (
          <Space size={4}>
            <Tooltip title={t('codebook.municipality.actions.edit')}>
              <Button type="text" icon={<EditOutlined />} size="small" onClick={() => openEditModal(rec)} />
            </Tooltip>
            <Tooltip title={t(rec.isActive ? 'codebook.municipality.actions.deactivate' : 'codebook.municipality.actions.activate')}>
              <Button type="text" icon={rec.isActive ? <StopOutlined /> : <CheckOutlined />}
                size="small" danger={rec.isActive} onClick={() => confirmToggleActive(rec)} />
            </Tooltip>
            <Tooltip title={t('codebook.municipality.actions.delete')}>
              <Button type="text" icon={<DeleteOutlined />} size="small" danger onClick={() => confirmDelete(rec)} />
            </Tooltip>
          </Space>
        )
      },
    },
  ], [t, openEditModal, confirmToggleActive, confirmDelete])

  return (
    <CodebookLayout
      title={t('codebook.municipality.title')}
      extra={
        <Space>
          <Select
            allowClear
            placeholder={t('codebook.municipality.filter_county')}
            value={countyFilter}
            onChange={(v) => setCountyFilter(v ?? null)}
            options={countyOptions}
            style={{ minWidth: 160 }}
            size="small"
          />
          <Checkbox checked={onlyActive} onChange={e => setOnlyActive(e.target.checked)}>
            {t('common.only_active')}
          </Checkbox>
          <Tooltip title={t('common.export_csv')}>
            <Button icon={<DownloadOutlined />} size="small" onClick={() => exportRef.current?.()} />
          </Tooltip>
          <Button type="primary" icon={<PlusOutlined />} onClick={openAddModal}>
            {t('codebook.municipality.addNew')}
          </Button>
        </Space>
      }
    >
      <AgGridWrapper<MunicipalityResponse>
        columnDefs={columnDefs}
        rowData={filteredData}
        loading={loading}
        exportRef={exportRef}
        getRowId={(p) => p.data.id}
        getRowStyle={(p) => p.data?.isActive ? undefined : { opacity: 0.45 }}
      />

      <AppModal
        title={editingItem ? t('codebook.municipality.modal.editTitle') : t('codebook.municipality.modal.addTitle')}
        open={modalOpen}
        onOk={handleSave}
        onCancel={() => setModalOpen(false)}
        okText={t('common.save')}
        cancelText={t('common.cancel')}
        confirmLoading={saving}
        destroyOnClose
      >
        <Form form={form} layout="vertical" style={{ marginTop: 16 }}>
          <Form.Item
            name="code"
            label={t('codebook.municipality.modal.code')}
            tooltip={t('codebook.municipality.modal.code_hint')}
            rules={[{ required: true, message: t('common.required') }]}
          >
            <Input maxLength={20} style={{ textTransform: 'uppercase' }} />
          </Form.Item>
          <Form.Item
            name="nameHr"
            label={t('codebook.municipality.modal.nameHr')}
            rules={[{ required: true, message: t('common.required') }]}
          >
            <Input maxLength={200} />
          </Form.Item>
          <Form.Item name="nameEn" label={t('codebook.municipality.modal.nameEn')}>
            <Input maxLength={200} />
          </Form.Item>
          <Form.Item name="countyId" label={t('codebook.municipality.modal.county')}>
            <Select allowClear options={countyOptions} />
          </Form.Item>
          <Form.Item name="ordinal" label={t('codebook.municipality.modal.ordinal')}>
            <InputNumber min={0} style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="isActive" label={t('codebook.municipality.modal.isActive')} valuePropName="checked">
            <Switch />
          </Form.Item>
        </Form>
      </AppModal>
    </CodebookLayout>
  )
}
